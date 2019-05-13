using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Helpers;
using Optimus.Model;

namespace Optimus
{
    public class FileTracker : IFileTracker
    {
        private const string FileName = "OptimusFileTracker.txt";
        private readonly string _trackerFile;
        private readonly string _directoryPath;
        private readonly IMediaSearch _mediaSearch;
        private readonly string[] _searchExtensions;
        private List<OptimusFileInfo> _currentReport;

        public FileTracker(string directoryPath, IMediaSearch mediaSearch)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                throw new ArgumentException(
                    $"Directory not found -> {directoryPath}",
                    nameof(directoryPath));
            }

            _directoryPath = directoryPath;
            
            _mediaSearch = mediaSearch ?? throw new ArgumentException(
                $"{nameof(IMediaSearch)} should be provided", 
                nameof(mediaSearch));
            
            _trackerFile = Path.Combine(directoryPath, FileName);
            
            _searchExtensions = OptimusConfiguration
                .GetOrCreate(_directoryPath)
                .configuration
                .FileExtensions;
        }

        public async Task<IEnumerable<OptimusFileInfo>> Report()
        {
            var trackedFileInfos = File.Exists(_trackerFile) 
                ? File
                    .ReadAllLines(_trackerFile)
                    .Select(ReadLine)
                    .ToList()
                : new List<OptimusFileInfo>();

            var trackedPaths = trackedFileInfos.Select(x => x.RelativePath);
            
            _currentReport = (await _mediaSearch.SearchMedia(_directoryPath, _searchExtensions))
                .Select(path => path.NormalizeSeparators())
                .Select(path =>
                {
                    var tracked = trackedPaths.Contains(path, StringComparer.OrdinalIgnoreCase);
                    return new OptimusFileInfo(path, tracked);
                })
                .ToList();

            // if any tracked files have been deleted, update tracker file now
            if (_currentReport.Count(x => x.Tracked) < trackedFileInfos.Count)
            {
                var lines = _currentReport
                    .Where(x => x.Tracked)
                    .Select(CreateLine);
                
                File.WriteAllLines(_trackerFile, lines);
            }

            return _currentReport;
        }

        private static OptimusFileInfo ReadLine(string line)
        {
            return new OptimusFileInfo(line, true);
        }

        private static string CreateLine(OptimusFileInfo optimusFileInfo)
        {
            return optimusFileInfo.RelativePath;
        }

        public string Track(string relativePath)
        {
            if(_currentReport == null)
                throw new InvalidOperationException(
                    $"Please call {nameof(Report)}() before tracking files");

            var normalizedPath = relativePath.NormalizeSeparators();

            if(_currentReport.All(x => x.RelativePath != normalizedPath))
                throw new ArgumentException(
                    "Cannot track a file not included in the Report", nameof(relativePath));

            var line = relativePath.NormalizeSeparators();
            // TODO format line with file timestamp
            // TODO check for duplicates
            File.AppendAllLines(_trackerFile, new []{line});
            return line;
        }

        public Task<IEnumerable<OptimusFileInfo>> SetAllFilesAsTracked()
        {
            throw new NotImplementedException();
        }

        public void Untrack()
        {
            _currentReport = null;
            
            if(File.Exists(_trackerFile))
                File.Delete(_trackerFile);
        }
    }
}
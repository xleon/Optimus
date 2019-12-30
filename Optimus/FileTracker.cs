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
        private const string DateTimeOffsetFormatString = "yyyy-MM-ddTHH:mm:sszzz";
        
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
            
            _currentReport = (await _mediaSearch.SearchMedia(_directoryPath, _searchExtensions))
                .Select(path => path.NormalizeSeparators())
                .Select(path =>
                {
                    var tracked = trackedFileInfos
                        .FirstOrDefault(x => x.RelativePath
                            .Equals(path, StringComparison.OrdinalIgnoreCase));

                    return tracked ?? new OptimusFileInfo(path);
                })
                .ToList();

            // if any tracked files have been deleted, update tracker file now
            if (_currentReport.Count(x => x.Tracked) < trackedFileInfos.Count)
            {
                var lines = _currentReport
                    .Where(x => x.Tracked)
                    .Select(CreateLine);
                
                await File.WriteAllLinesAsync(_trackerFile, lines);
            }

            return _currentReport;
        }

        private static OptimusFileInfo ReadLine(string line)
        {
            var parts = line.Substring(1).Split("] ");
            var optimizedAt = DateTimeOffset.Parse(parts[0]);
            var path = parts[1];
            
            return new OptimusFileInfo(path, true, optimizedAt);
        }
        
        private static string CreateLine(OptimusFileInfo optimusFileInfo)
        {
            if(optimusFileInfo.OptimizedAt == null)
                throw new InvalidDataException($"{nameof(optimusFileInfo.OptimizedAt)} property is missing");
            
            var optimizedAt = optimusFileInfo.OptimizedAt.Value.ToString(DateTimeOffsetFormatString);
            return $"[{optimizedAt}] {optimusFileInfo.RelativePath}";
        }

        public async Task<OptimusFileInfo> Track(string relativePath)
        {
            if(_currentReport == null)
                throw new InvalidOperationException(
                    $"Please call {nameof(Report)}() before tracking files");

            var normalizedPath = relativePath.NormalizeSeparators();

            if(_currentReport.All(x => x.RelativePath != normalizedPath))
                throw new InvalidOperationException(
                    $"Cannot track a file that is not included in the report: {relativePath}");

            var fileInfo = new OptimusFileInfo(normalizedPath, true, DateTimeOffset.UtcNow);
            var line = CreateLine(fileInfo);
      
            await File.AppendAllLinesAsync(_trackerFile, new []{line});
            
            return fileInfo;
        }

        public async Task<IEnumerable<OptimusFileInfo>> AsumeAllFilesAlreadyTracked()
        {
            var report = _currentReport ?? await Report();
            
            var untracked = report
                .Where(x => !x.Tracked)
                .Select(x => new OptimusFileInfo(x.RelativePath, true, DateTimeOffset.UtcNow))
                .ToList();
            
            await File.WriteAllLinesAsync(_trackerFile, untracked.Select(CreateLine));
            
            return untracked;
        }

        public void Untrack()
        {
            _currentReport = null;
            
            if(File.Exists(_trackerFile))
                File.Delete(_trackerFile);
        }
    }
}
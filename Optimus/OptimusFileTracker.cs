using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Model;

namespace Optimus
{
    public class OptimusFileTracker
    {
        private readonly string _fileName = $"{nameof(OptimusFileTracker)}.txt";
        private readonly string _trackerFile;
        private readonly string _directoryPath;
        private readonly IMediaSearch _mediaSearch;

        public OptimusFileTracker(string directoryPath, IMediaSearch mediaSearch)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                throw new ArgumentException(
                    $"Directory not found -> {directoryPath}",
                    nameof(directoryPath));
            }

            _directoryPath = directoryPath;
            _mediaSearch = mediaSearch;
            _trackerFile = Path.Combine(directoryPath, _fileName);
        }

        public async Task<(IEnumerable<string> trackedFiles, IEnumerable<string> untrackedfiles)> GetFiles()
        {
            var config = OptimusConfiguration.GetOrCreate(_directoryPath);
            var trackedFiles = ReadTrackedFiles().ToList();
            var trackedCount = trackedFiles.Count;
            var directoryFiles = (await _mediaSearch.SearchImageFiles(
                    _directoryPath,
                    config.FileExtensions,
                    config.IncludedDirectories))
                .ToList();

            if (trackedFiles.Any() && directoryFiles.Any())
            {
                trackedFiles = trackedFiles
                    .Where(directoryFiles.Contains)
                    .ToList();

                if (trackedFiles.Count != trackedCount)
                {
                    // if we removed any of the tracked files because they don´t exist any more in the 
                    // file system, overwrite the tracker file to reflect changes
                    File.WriteAllLines(_trackerFile, trackedFiles);
                }
            }

            var untrackedFiles = directoryFiles
                .Where(x => !trackedFiles.Contains(x));

            return (trackedFiles, untrackedFiles);
        }

        public void Track(string file)
        {
            File.AppendAllLines(_trackerFile, new []{file});
        }

        private IEnumerable<string> ReadTrackedFiles()
        {
            if (!File.Exists(_trackerFile))
            {
                File.WriteAllText(_trackerFile, string.Empty);
                return Enumerable.Empty<string>();
            }

            return File.ReadLines(_trackerFile);
        }
    }
}
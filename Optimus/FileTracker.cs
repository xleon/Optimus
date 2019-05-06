using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Model;

namespace Optimus
{
    public class FileTracker : IFileTracker
    {
        private readonly string _fileName = $"{nameof(FileTracker)}.txt";
        private readonly string _trackerFile;
        private readonly string _directoryPath;
        private readonly IMediaSearch _mediaSearch;

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
            
            _trackerFile = Path.Combine(directoryPath, _fileName);
        }

        public async Task<IEnumerable<OptimusFile>> GetFiles()
        {
            return null;
        }

        public void Track(OptimusFile optimusFile)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OptimusFile>> SetAllFilesAsTracked()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            // throw new NotImplementedException();
        }

//        public async Task<(IEnumerable<string> trackedFiles, IEnumerable<string> untrackedfiles)> GetFiles()
//        {
//            var (config, created) = OptimusConfiguration.GetOrCreate(_directoryPath);
//            var trackedFiles = ReadTrackedFiles().ToList();
//            var trackedCount = trackedFiles.Count;
//            var directoryFiles = (await _mediaSearch.SearchMedia(
//                    _directoryPath,
//                    config.FileExtensions))
//                .ToList();
//
//            if (trackedFiles.Any() && directoryFiles.Any())
//            {
//                trackedFiles = trackedFiles
//                    .Where(directoryFiles.Contains)
//                    .ToList();
//
//                if (trackedFiles.Count != trackedCount)
//                {
//                    // if we removed any of the tracked files because they don´t exist any more in the 
//                    // file system, overwrite the tracker file to reflect changes
//                    File.WriteAllLines(_trackerFile, trackedFiles);
//                }
//            }
//
//            var untrackedFiles = directoryFiles
//                .Where(x => !trackedFiles.Contains(x));
//
//            return (trackedFiles, untrackedFiles);
//        }

    }
}
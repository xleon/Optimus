using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Model;

namespace Optimus
{
    public class OptimusFileTracker : IOptimusFileTracker
    {
        public const string FileName = "OptimusFileTracker.txt";
        
        private readonly string _trackerFile;
        private readonly string _directoryPath;
        
        public OptimusFileTracker(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                throw new ArgumentException(
                    $"Directory not found -> {directoryPath}",
                    nameof(directoryPath));
            }

            _directoryPath = directoryPath;
            _trackerFile = Path.Combine(directoryPath, FileName);
        }

        public FileInfo GetTrackerFileInfo() 
            => new FileInfo(_trackerFile);

        public async Task<IEnumerable<TrackInfo>> GetTrackInfos()
        {
            if(!File.Exists(_trackerFile))
                return new List<TrackInfo>();

            var lines = await File.ReadAllLinesAsync(_trackerFile);
            var infos = lines.Select(ReadLine);

            return infos;
        }

        public async Task<TrackInfo> Track(string relativePath)
        {
            var absolutePath = Path.Combine(_directoryPath, relativePath);
            var file = new FileInfo(absolutePath);
            
            if(!file.Exists)
                throw new FileNotFoundException(
                    $"The provided path does not point to an existing file", 
                    absolutePath);
            
            var trackInfos = (await GetTrackInfos())
                .ToList();

            var match = trackInfos
                .FirstOrDefault(x => x.RelativePath == relativePath);
            
            if(match == null)
            {
                match = new TrackInfo(file.LastWriteTimeUtc, relativePath) {Updated = true};
                trackInfos.Add(match);
            }
            else if (match.LastWriteTimeUtc == file.LastWriteTimeUtc)
            {
                // assume the file didn't change and it's already tracked
                return match;
            }
            else
            {
                match.LastWriteTimeUtc = file.LastWriteTimeUtc;
                match.Updated = true;
            }
            
            await File
                .WriteAllLinesAsync(_trackerFile, trackInfos
                    .OrderBy(x => x.RelativePath)
                    .Select(CreateLine));
            
            return match;
        }

        public async Task UntrackRemovedAndChangedFiles()
        {
            var tracked = (await GetTrackInfos()).ToList();
            
            var updatedLines = tracked
                .Where(x =>
                {
                    var path = Path.Combine(_directoryPath, x.RelativePath);
                    var fileInfo = new FileInfo(path);
                    
                    return fileInfo.Exists && fileInfo.LastWriteTimeUtc == x.LastWriteTimeUtc;
                })
                .OrderBy(x => x.RelativePath)
                .Select(CreateLine)
                .ToList();

            if (tracked.Count == updatedLines.Count)
                return;
            
            await File.WriteAllLinesAsync(_trackerFile, updatedLines);
        }

        public bool UntrackAll()
        {
            if(File.Exists(_trackerFile))
            {
                File.Delete(_trackerFile);
                return true;
            }

            return false;
        }
        
        private static TrackInfo ReadLine(string line)
        {
            var parts = line.Substring(1).Split("] ");
            var lastWriteTimeUtc = DateTime.Parse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var path = parts[1];

            return new TrackInfo(lastWriteTimeUtc, path);
        }
        
        private static string CreateLine(TrackInfo trackInfo)
        {
            var serializedLastWriteTimeUtc = trackInfo.LastWriteTimeUtc.ToString("O");
            return $"[{serializedLastWriteTimeUtc}] {trackInfo.RelativePath}";
        }
    }
}
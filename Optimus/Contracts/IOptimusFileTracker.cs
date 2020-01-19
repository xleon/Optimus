using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Optimus.Model;

namespace Optimus.Contracts
{
    public interface IOptimusFileTracker
    {
        FileInfo GetTrackerFileInfo();
        Task<IEnumerable<TrackInfo>> GetTrackInfos();
        Task<IEnumerable<string>> GetUntrackedPaths(IEnumerable<string> allPaths);
        Task<TrackInfo> Track(string relativePath);
        Task UntrackRemovedAndChangedFiles();
        bool UntrackAll();
    }
}
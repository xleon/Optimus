using System;

namespace Optimus.Model
{
    public class TrackInfo
    {
        public string RelativePath { get; }
        public DateTime LastWriteTimeUtc { get; internal set; }
        public bool Updated { get; internal set; }
        
        public TrackInfo(DateTime lastWriteTimeUtc, string relativePath)
        {
            LastWriteTimeUtc = lastWriteTimeUtc;
            RelativePath = relativePath;
        }
    }
}
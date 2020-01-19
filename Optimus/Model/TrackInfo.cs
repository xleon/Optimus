namespace Optimus.Model
{
    public class TrackInfo
    {
        public string RelativePath { get; }
        public string FileHash { get; internal set; }
        public bool Updated { get; internal set; }
        
        public TrackInfo(string fileHash, string relativePath)
        {
            FileHash = fileHash;
            RelativePath = relativePath;
        }
    }
}
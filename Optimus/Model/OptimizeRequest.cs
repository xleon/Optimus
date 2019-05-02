using System.IO;

namespace Optimus.Model
{
    public class OptimizeRequest
    {
        public string FilePath { get; }
        public long Length { get; }

        public OptimizeRequest(string filePath)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            
            FilePath = filePath;
            Length = new FileInfo(filePath).Length;
        }
    }
}
using System.IO;

namespace Optimus.Model
{
    public class OptimizeResult
    {
        public bool Success { get; }
        public string FilePath { get; }
        public long OriginalLength { get; }
        public string ErrorMessage { get; }
        public long Length { get; }

        public OptimizeResult(bool success, string filePath, long? originalLength = null, string errorMessage = null)
        {
            if(success && !File.Exists(filePath))
                throw new FileNotFoundException(filePath);
            
            Success = success;
            FilePath = filePath;
            OriginalLength = originalLength ?? 0;
            ErrorMessage = errorMessage;
            Length = new FileInfo(filePath).Length;
        }
    }
}
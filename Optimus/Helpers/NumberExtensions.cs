namespace Optimus.Helpers
{
    public static class NumberExtensions
    {
        public static string ToFileLengthRepresentation(this long fileLength)
        {
            if (fileLength >= 1 << 30)
                return $"{fileLength >> 30} GB";
            
            if (fileLength >= 1 << 20)
                return $"{fileLength >> 20} MB";
            
            if (fileLength >= 1 << 10)
                return $"{fileLength >> 10} KB";

            return $"{fileLength} B";
        }
    }
}
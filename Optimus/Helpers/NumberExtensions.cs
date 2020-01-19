namespace Optimus.Helpers
{
    public static class NumberExtensions
    {
        public static string ToFileLengthRepresentation(this long fileLength)
        {
            if (fileLength >= 1 << 30)
                return $"{fileLength >> 30}Gb";
            
            if (fileLength >= 1 << 20)
                return $"{fileLength >> 20}Mb";
            
            if (fileLength >= 1 << 10)
                return $"{fileLength >> 10}Kb";

            return $"{fileLength}B";
        }
    }
}
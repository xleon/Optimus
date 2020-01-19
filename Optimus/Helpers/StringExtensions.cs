namespace Optimus.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeSeparators(this string @this)
            => @this.Replace("\\", "/").Replace("//", "/");
    }
}
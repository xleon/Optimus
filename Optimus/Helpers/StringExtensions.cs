namespace Optimus.Helpers
{
    internal static class StringExtensions
    {
        public static string NormalizeSeparators(this string @this)
            => @this.Replace("\\", "/");
    }
}
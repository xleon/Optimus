using System;
using System.IO;
using System.Security.Cryptography;

namespace Optimus.Helpers
{
    public static class FileExtensions
    {
        public static string CalculateMd5(this string filename)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
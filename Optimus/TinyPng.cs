using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinifyAPI;
using Exception = TinifyAPI.Exception;

namespace Optimus
{
    public class TinyPng : IOptimizer
    {
        private readonly string[] _apiKeys;

        public TinyPng(string[] apiKeys)
        {
            if(_apiKeys == null || !apiKeys.Any())
            {
                throw new ArgumentException(
                    $"{nameof(apiKeys)} cannot be null or empty", 
                    nameof(apiKeys));
            }
            
            _apiKeys = apiKeys;
        }

        public async Task Initialize()
        {
            foreach (var key in _apiKeys)
            {
                try
                {
                    Tinify.Key = key;
                    await Tinify.Validate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    if (key.Equals(_apiKeys.Last()))
                        throw;
                }
            }
        }
        
        public async Task<OptimusResult> Optimize(string sourcePath, string targetPath)
        {
            var result = new OptimusResult();
            
            try
            {
                var source = Tinify.FromFile(sourcePath);
                await source.ToFile(targetPath);
            }
            catch (AccountException e)
            {
                Console.WriteLine("The error message is: " + e.Message);
                // Verify your API key and account limit.
            }
            catch (ClientException e)
            {
                // Check your source image and request options.
            }
            catch (ServerException e)
            {
                // Temporary issue with the Tinify API.
            }
            catch (ConnectionException e)
            {
                // A network connection error occurred.
            }
            catch (System.Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
            }

            return result;
        }
    }

    public class OptimusResult
    {
        public bool Success { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public string SourcePath { get; internal set; }
        public string TargetPath { get; internal set; }
        
        public long OriginalSize 
            => Success && File.Exists(SourcePath) ? new FileInfo(SourcePath).Length : 0;
        public long OptimizedSize 
            => Success && File.Exists(TargetPath) ? new FileInfo(TargetPath).Length : 0;

        public long OptimizationPercentage
            => Success ? OptimizedSize / OriginalSize : 0;
    }
}
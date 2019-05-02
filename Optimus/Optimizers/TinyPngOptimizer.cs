using System;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Exceptions;
using Optimus.Model;
using Serilog;
using TinifyAPI;
using Exception = System.Exception;

namespace Optimus.Optimizers
{
    public class TinyPngOptimizer : IOptimizer
    {
        private string[] _apiKeys;
        private readonly ILogger _logger;

        public TinyPngOptimizer(string[] apiKeys)
        {
            _logger = Log.ForContext<TinyPngOptimizer>();
            
            if(apiKeys == null || !apiKeys.Any())
            {
                throw new ArgumentException(
                    $"{nameof(apiKeys)} cannot be null or empty", 
                    nameof(apiKeys));
            }

            if (apiKeys.Any(key => string.IsNullOrEmpty(key.Trim())))
            {
                throw new ArgumentException(
                    "One more more keys are empty", 
                    nameof(apiKeys));
            }
            
            _apiKeys = apiKeys.Select(x => x.Trim()).ToArray();

            Tinify.Key = _apiKeys.First();
        }

        public async Task Initialize()
        {
            foreach (var key in _apiKeys)
            {
                try
                {
                    _logger.Information($"Validating TinyPng api key {key}...");
                    
                    Tinify.Key = key;
                    await Tinify.Validate();
                    
                    _logger.Information("Api key is working!");
                    _logger.Information($"Compression count this month with key {key}: {Tinify.CompressionCount}");
                }
                catch (Exception e)
                {
                    _logger.Error($"TinyPng api key {key} not valid => {e.Message}");

                    if (!key.Equals(_apiKeys.Last())) 
                        continue;
                    
                    _logger.Error("Could not initialize TinyPng because none of the provided api keys could be validated");
                    throw new ApiAccessException(e.Message);
                }
            }
        }
        
        public async Task<OptimizeResult> Optimize(OptimizeRequest request)
        {
            try
            {
                var source = Tinify.FromFile(request.FilePath);
                await source.ToFile(request.FilePath);
                
                return new OptimizeResult(true, request.FilePath, request.Length);
            }
            catch (AccountException e)
            {
                // Verify your API key and account limit.

                _apiKeys = _apiKeys.Where(x => x != Tinify.Key).ToArray();
                var otherApiKeys = _apiKeys.ToList();
                
                if(!otherApiKeys.Any())
                    throw new ApiAccessException(e.Message);

                string errorMessage = null;
                
                foreach (var key in otherApiKeys)
                {
                    try
                    {
                        _logger.Information($"Validating TinyPng api key {key}...");
                        
                        Tinify.Key = key;
                        await Tinify.Validate();
                        
                        _logger.Information("Api key is working!");
                        _logger.Information($"[{nameof(TinyPngOptimizer)}] " +
                                        $"Compression count with key {key}: {Tinify.CompressionCount}");

                        return await Optimize(request);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        _logger.Error($"TinyPng api key {key} not valid => {e.Message}");
                        
                        if (key.Equals(otherApiKeys.Last()))
                        {
                            Log.Error("Could not initialize TinyPng because none of the provided api keys could be validated");
                        }
                    }
                }
                
                throw new ApiAccessException(errorMessage);
            }
            // TODO retry policy for ServerException and ConnectionException
//            catch (ServerException e)
//            {
//                // Temporary issue with the Tinify API.
//            }
//            catch (ConnectionException e)
//            {
//                // A network connection error occurred.
//            }
            catch (Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
                return new OptimizeResult(false, request.FilePath, errorMessage: e.Message);
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Exceptions;
using Optimus.Model;
using Polly;
using Polly.Retry;
using Serilog;
using TinifyAPI;
using Exception = System.Exception; // Tinify library has a class called "Exception" WTF

namespace Optimus.Optimizers
{
    public class TinyPngOptimizer : IOptimizer
    {
        private string[] _apiKeys;
        private readonly ILogger _logger;
        private readonly AsyncRetryPolicy _policy;

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
            
            _policy = Policy
                .Handle<Exception>(ex => ex is ServerException || ex is ConnectionException)
                .WaitAndRetryAsync(6, 
                    attempt => TimeSpan.FromSeconds(0.1 * Math.Pow(2, attempt)), // Back off!  2, 4, 8, 16 etc times 1/4-second
                    (exception, calculatedWaitDuration) =>  
                    {
                        _logger.Warning(exception.Message);
                        _logger.Warning($"Delaying retry for {calculatedWaitDuration.TotalMilliseconds} ms...");
                    });
        }

        public async Task Initialize()
        {
            foreach (var key in _apiKeys)
            {
                try
                {
                    _logger.Information($"Validating TinyPng api key {key}...");
                    
                    Tinify.Key = key;
                    await _policy.ExecuteAsync(Tinify.Validate);
                    
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

                await _policy.ExecuteAsync(() => source.ToFile(request.FilePath));
                
                return new OptimizeResult(true, request.FilePath, request.Length);
            }
            catch (AccountException e)
            {
                // This happens if your API key is wrong or you´ve reached account limits

                _apiKeys = _apiKeys.Where(x => x != Tinify.Key).ToArray();

                if(!_apiKeys.Any())
                    throw new ApiAccessException(e.Message);

                string errorMessage = null;
                
                foreach (var key in _apiKeys)
                {
                    try
                    {
                        _logger.Information($"Validating TinyPng api key {key}...");
                        
                        Tinify.Key = key;
                        await _policy.ExecuteAsync(Tinify.Validate);
                        
                        _logger.Information("Api key is working!");
                        _logger.Information($"[{nameof(TinyPngOptimizer)}] " +
                                        $"Compression count with key {key}: {Tinify.CompressionCount}");

                        return await Optimize(request);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                        _logger.Error($"TinyPng api key {key} not valid => {e.Message}");
                        
                        if (key.Equals(_apiKeys.Last()))
                        {
                            Log.Error("Could not initialize TinyPng because none of the provided api keys could be validated");
                        }
                    }
                }
                
                throw new ApiAccessException(errorMessage);
            }
            catch (Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
                return new OptimizeResult(false, request.FilePath, errorMessage: e.Message);
            }
        }
    }
}
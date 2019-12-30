using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Exceptions;
using Optimus.Model;
using Polly;
using Polly.Retry;
using Serilog;
using TinifyAPI;
using Exception = System.Exception; // Tinify library has a class called "Exception". WTF?

namespace Optimus
{
    public class TinyPngOptimizer : IOptimizer
    {
        private readonly ILogger _logger;
        private readonly Queue<string> _apiKeys;
        private readonly AsyncRetryPolicy _networkPolicy;
        private readonly AsyncRetryPolicy _accountPolicy;

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
                    "One or more keys are empty", 
                    nameof(apiKeys));
            }
            
            _apiKeys = new Queue<string>(apiKeys.Select(x => x.Trim()));
            
            _networkPolicy = Policy
                .Handle<ServerException>()
                .Or<ConnectionException>()
                .WaitAndRetryAsync(6, 
                    attempt => TimeSpan.FromSeconds(0.2 * Math.Pow(2, attempt)),
                    (exception, calculatedWaitDuration) =>  
                    {
                        _logger.Warning(exception.Message);
                        _logger.Warning($"Delaying retry for {calculatedWaitDuration.TotalMilliseconds} ms...");
                    });
            
            _accountPolicy = Policy
                .Handle<AccountException>()
                .RetryAsync((exception, calculatedWaitDuration) => Validate());
        }
        
        private async Task Validate()
        {
            while (_apiKeys.TryDequeue(out var key))
            {
                try
                {
                    Tinify.Key = key;
                    
                    await _networkPolicy.ExecuteAsync(Tinify.Validate);
                    
                    _logger.Information($"Using api key {key}. Compressions this month: {Tinify.CompressionCount}");
                    
                    return;
                }
                catch (Exception e)
                {
                    _logger.Error($"TinyPng api key '{key}' could not be validated due to: {e.Message}");
                }
            }
            
            throw new OptimusApiAccessException("Could not initialize TinyPng because " +
                "none of the provided api keys could be validated");
        }
        
        public async Task<OptimizeResult> Optimize(OptimizeRequest request)
        {
            try
            {
                await _networkPolicy
                    .WrapAsync(_accountPolicy)
                    .ExecuteAsync(() =>
                {
                    var source = Tinify.FromFile(request.FilePath);
                    return source.ToFile(request.FilePath);
                });

                return new OptimizeResult(true, request.FilePath, request.Length);
            }
            catch (OptimusApiAccessException)
            {
                throw;
            }
            catch (Exception e)
            {
                // Something else went wrong, unrelated to the Tinify API.
                return new OptimizeResult(false, request.FilePath, request.Length, e.Message);
            }
        }
    }
}
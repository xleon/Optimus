using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ByteSizeLib;
using Optimus;
using Optimus.Contracts;
using Optimus.Helpers;
using Optimus.Model;
using Serilog;

namespace OptimusTool
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var exitCode = await Run(args);
            
            Log.CloseAndFlush();

            return exitCode;
        }

        private static async Task<int> Run(string[] args)
        {
            try
            {
                var mediaSearch = new GitMediaSearch();
                var (directory, config) = await CheckTargetDirectory(args, mediaSearch);
                var media = (await mediaSearch.SearchMedia(directory, config.FileExtensions)).ToList();
                
                if(!media.Any())
                {
                    Log.Information("Nothing to optimise. Add images to your project and commit them to git and then run this command again");
                    return 0;
                }

                var tracker = new OptimusFileTracker(directory);
                await tracker.UntrackRemovedAndChangedFiles();

                var untrackedPaths = (await tracker.GetUntrackedPaths(media)).ToList();
                
                if(!untrackedPaths.Any())
                {
                    Log.Information("All images in this repo are already optimised and tracked");
                    return 0;
                }
                
                var imagesStr = untrackedPaths.Count == 1 ? "image" : "images";
                Log.Information($"Starting optimisation for {untrackedPaths.Count} {imagesStr}...");

                var optimizer = new TinyPngOptimizer(config.TinyPngApiKeys);
                long reduceAggregation = 0;
                var errored = 0;
                var succeded = 0;
                var count = 1;
                var total = untrackedPaths.Count;
                
                foreach (var path in untrackedPaths)
                {
                    var absolutePath = Path.Combine(directory, path);
                    var request = new OptimizeRequest(absolutePath);
                    var result = await optimizer.Optimize(request);

                    if (!result.Success)
                    {
                        Log.Error($"{path}: {result.ErrorMessage}");
                        errored++;
                        continue;
                    }

                    reduceAggregation += result.OriginalLength - result.Length;
                    
                    await tracker.Track(path.NormalizeSeparators());

                    succeded++;

                    var progress = $"{count}/{total}";
                    var sizeComparison = $"{ByteSize.FromBytes(result.OriginalLength).ToString("KB")} > {ByteSize.FromBytes(result.Length).ToString("KB")}";
                    var percentage = $"{(decimal) result.Length / result.OriginalLength:P}";
                    var sizeString = $"{progress} [{sizeComparison} ({percentage} off original size)]";
                    var message = $"{sizeString.PadRight(60)} {path}";
                    
                    Log.Information(message);

                    count++;
                }
                
                Log.Information("------------------------------------------------");
                Log.Information($"{succeded} {imagesStr} optimised");
                Log.Information($"{errored} optimizations failed");
                Log.Information($"Overall project size reduction: {reduceAggregation.ToFileLengthRepresentation()}");

                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return 2;
            }
        }

        private static async Task<(string directory, OptimusConfiguration config)> CheckTargetDirectory(string[] args, IMediaSearch mediaSearch)
        {
            var directory = GetDirectory(args);

            await mediaSearch.CheckSearchDirectory(directory);
            
            var (config, created) = OptimusConfiguration.GetOrCreate(directory);

            if (created)
            {
                var message =
                    "Configuration file was missing. " +
                    $"{Environment.NewLine}" +
                    $"A configuration file has been created at {config.ConfigurationFilePath}. " +
                    $"{Environment.NewLine}" +
                    "Important! this file should be added to your git repo. " +
                    $"{Environment.NewLine}" +
                    "Please edit the file with your API keys and image search extensions." +
                    $"{Environment.NewLine}" +
                    "You can get a TinyPNG API key at https://tinypng.com/developers";
                
                throw new InvalidOperationException(message);
            }
                
            return (directory, config);
        }

        private static string GetDirectory(string[] args)
        {
            if (args == null || args.Length == 0)
                return Directory.GetCurrentDirectory();
            
            if (!Directory.Exists(args[0]))
                throw new DirectoryNotFoundException();

            return args[0];
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Optimus;
using Optimus.Contracts;
using Optimus.Model;
using Serilog;

namespace OptimusTool
{
    static class Program
    {
        /// <summary>
        /// $optimus
        /// - ✔ check if the current directory is a git repo, show error otherwise
        /// - ✔ check configuration file: if it was created right now or it's lacking api keys, show message (and help to create a TinyPng apikey)
        /// - ✔ search all media files
        /// - ✔ if no media files show message "Nothing to optimise. Add images to your project and run this command again"
        /// - check tracked files
        /// - if all media is tracked show message "All images in this repo are already optimised"
        /// - show total number of images that will be optimised
        /// - start optimising and tracking 1 by 1: show file size percentage saved (eg: 1/20 path/to/file 35% off original size
        /// - end: show percentage saved overall
        ///
        /// $optimus -force
        /// - search media files
        /// - ask question: Do you wish to force tracking of all images?
        /// - add all media files to tracker file
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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
                    throw new InvalidOperationException("Nothing to optimise. Add images to your project and run this command again");

                return 0;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return 1;
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
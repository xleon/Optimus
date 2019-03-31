using System;
using System.Collections.Generic;
using System.IO;

namespace Optimus
{
    public class OptimusRuner
    {
        private readonly IOptimizer _optimizer;

        public OptimusRuner(IOptimizer optimizer)
        {
            _optimizer = optimizer ?? throw new ArgumentException(nameof(optimizer));
        }

        public async void Run(IEnumerable<string> untrackedfiles)
        {
            //SanityCheck(directoryPath);
            
            foreach (var file in untrackedfiles)
            {
                var temp = Path.Combine(Path.GetTempPath(), file);

                try
                {
                    var result = await _optimizer.Optimize(file, file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void SanityCheck(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
            {
                throw new ArgumentException(
                    $"Directory not found -> {directoryPath}",
                    nameof(directoryPath));
            }
            
            var config = OptimusConfiguration.GetOrCreate(directoryPath);

            
        }
    }
}
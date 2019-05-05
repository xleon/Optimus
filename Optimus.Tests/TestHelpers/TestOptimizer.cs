using System;
using System.Threading.Tasks;
using Optimus.Contracts;
using Optimus.Model;

namespace Optimus.Tests.TestHelpers
{
    public class TestOptimizer : IOptimizer
    {
        private static readonly Random Random = new Random();

        public Task Validate()
        {
            return Task.FromResult(0);
        }

        public async Task<OptimizeResult> Optimize(OptimizeRequest request)
        {
            await Task.Delay(10);
            
            // File.Copy(sourcePath, targetPath, true);

            return null;
        }
    }
}
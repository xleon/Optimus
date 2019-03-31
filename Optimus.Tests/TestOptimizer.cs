using System.IO;
using System.Threading.Tasks;

namespace Optimus.Tests
{
    public class TestOptimizer : IOptimizer
    {
        public async Task<OptimusResult> Optimize(string sourcePath, string targetPath)
        {
            await Task.Delay(2500);
            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }
    }
}
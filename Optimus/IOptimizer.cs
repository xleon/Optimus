using System.Threading.Tasks;

namespace Optimus
{
    public interface IOptimizer
    {
        Task<OptimusResult> Optimize(string sourcePath, string targetPath);
    }
}
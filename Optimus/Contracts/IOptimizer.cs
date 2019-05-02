using System.Threading.Tasks;
using Optimus.Model;

namespace Optimus.Contracts
{
    public interface IOptimizer
    {
        Task<OptimizeResult> Optimize(OptimizeRequest request);
    }
}
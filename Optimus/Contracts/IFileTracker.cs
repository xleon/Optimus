using System.Collections.Generic;
using System.Threading.Tasks;
using Optimus.Model;

namespace Optimus.Contracts
{
    public interface IFileTracker
    {
        Task<IEnumerable<OptimusFileInfo>> Report();
        Task<OptimusFileInfo> Track(string relativePath);
        Task<IEnumerable<OptimusFileInfo>> AsumeAllFilesAlreadyTracked();
        void Untrack();
    }
}
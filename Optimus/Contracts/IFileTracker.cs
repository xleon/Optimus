using System.Collections.Generic;
using System.Threading.Tasks;
using Optimus.Model;

namespace Optimus.Contracts
{
    public interface IFileTracker
    {
        Task<IEnumerable<OptimusFileInfo>> Report();
        string Track(string relativePath);
        Task<IEnumerable<OptimusFileInfo>> SetAllFilesAsTracked();
        void Untrack();
    }
}
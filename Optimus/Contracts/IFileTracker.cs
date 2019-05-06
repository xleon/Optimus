using System.Collections.Generic;
using System.Threading.Tasks;
using Optimus.Model;

namespace Optimus.Contracts
{
    public interface IFileTracker
    {
        Task<IEnumerable<OptimusFile>> GetFiles();
        void Track(OptimusFile optimusFile);
        Task<IEnumerable<OptimusFile>> SetAllFilesAsTracked();
        void Delete();
    }
}
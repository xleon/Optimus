using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimus.Contracts
{
    public interface IMediaSearch
    {
        Task<IEnumerable<string>> SearchMedia(
            string directory, 
            string[] extensions);
    }
}
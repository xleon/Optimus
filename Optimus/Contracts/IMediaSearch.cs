using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimus.Contracts
{
    public interface IMediaSearch
    {
        Task<IEnumerable<string>> SearchImageFiles(
            string path, 
            string[] extensions,
            string[] includeDirectories = null);
    }
}
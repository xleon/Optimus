using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optimus
{
    public interface IMediaSearch
    {
        Task<IEnumerable<string>> SearchImageFiles(
            string path, 
            string[] extensions,
            string[] includeDirectories = null);
    }
}
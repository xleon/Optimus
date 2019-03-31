using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Optimus
{
    public class GitMediaSearch : IMediaSearch
    {
        public async Task<IEnumerable<string>> SearchImageFiles(
            string path, 
            string[] extensions,
            string[] includeDirectories = null)
        {
            var files = (await GetFilesFromGitLs(path)).ToList();

            if (!files.Any())
                return Enumerable.Empty<string>();
            
            var filtered = files.Where(file => extensions.Contains(Path.GetExtension(file)));

            if (includeDirectories != null && includeDirectories.Any())
                filtered = filtered.Where(x => includeDirectories.Any(x.StartsWith));

            return filtered;
        }

        private static async Task<IEnumerable<string>> GetFilesFromGitLs(string path)
        {
            using (var powershell = PowerShell.Create()) 
            {
                powershell.AddScript($@"cd {path}");
                powershell.AddScript(@"git ls-files");

                var result = await powershell.InvokeAsync();
                return result.Select(x => x.ToString());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Optimus.Contracts;

namespace Optimus
{
    public class GitMediaSearch : IMediaSearch
    {
        public Task CheckSearchDirectory(string directory)
        {
            if(!Directory.Exists(directory))
                throw new DirectoryNotFoundException();
            
            if(!Directory.Exists(Path.Combine(directory, ".git")))
                throw new InvalidOperationException($"{directory} is not a git repository");

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> SearchMedia(
            string directory, 
            string[] extensions)
        {
            await CheckSearchDirectory(directory);
            
            if(extensions == null || !extensions.Any() || !extensions.All(x => x.StartsWith(".")))
                throw new ArgumentException("Incorrect extensions: " +
                    "At least one extension should be provided and it should start with '.'", 
                    nameof(extensions));
            
            var files = await GetFilesFromGitLs(directory);
            
            return files
                .Where(file => extensions.Contains(
                    Path.GetExtension(file), 
                    StringComparer.OrdinalIgnoreCase));
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
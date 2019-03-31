using System.IO;

namespace Optimus
{
    public class OptimusConfiguration
    {
        public string[] TinyPngApiKeys { get; set; }
        public string[] FileExtensions { get; set; }
        public string[] IncludedDirectories { get; set; }

        private static readonly string FileName = $"{nameof(OptimusConfiguration)}.json";

        public static OptimusConfiguration GetOrCreate(string path)
        {
            var configurationPath = Path.Combine(path, FileName);

            if (!File.Exists(configurationPath))
            {
                var configuration = new OptimusConfiguration
                {
                    TinyPngApiKeys = new string[]{},
                    IncludedDirectories = new string[]{},
                    FileExtensions = new []{".jpg", ".jpeg", ".png"}
                };
                
                configuration.SaveTo(path);

                return configuration;
            }
            
            return JsonFiles.ReadJson<OptimusConfiguration>(configurationPath);
        }

        public void SaveTo(string path)
        {
            var configurationPath = Path.Combine(path, FileName);
            JsonFiles.WriteJson(this, configurationPath);
        }
    }
}
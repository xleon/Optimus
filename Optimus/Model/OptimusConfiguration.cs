using System.IO;
using Newtonsoft.Json;
using Optimus.Exceptions;
using Optimus.Helpers;

namespace Optimus.Model
{
    public class OptimusConfiguration
    {
        public string[] TinyPngApiKeys { get; set; }
        public string[] FileExtensions { get; set; }
        
        [JsonIgnore]
        public string ConfigurationFilePath { get; private set; }

        private static readonly string FileName = $"{nameof(OptimusConfiguration)}.json";

        public static (OptimusConfiguration configuration, bool created) GetOrCreate(string directoryPath)
        {
            var configurationPath = Path.Combine(directoryPath, FileName);

            if (!File.Exists(configurationPath))
            {
                var configuration = new OptimusConfiguration
                {
                    TinyPngApiKeys = new string[]{},
                    FileExtensions = new []{".jpg", ".jpeg", ".png"},
                    ConfigurationFilePath = configurationPath
                };
                
                configuration.Save(directoryPath);

                return (configuration, true);
            }
            
            var savedConfiguration = JsonFile.Read<OptimusConfiguration>(configurationPath);
            
            if (savedConfiguration.FileExtensions.IsNullOrEmpty())
                throw new OptimusConfigurationException($"{nameof(FileExtensions)} missing");
            
            if (savedConfiguration.TinyPngApiKeys.IsNullOrEmpty())
                throw new OptimusConfigurationException($"{nameof(TinyPngApiKeys)} missing");

            return (savedConfiguration, false);
        }

        public void Save(string directoryPath) 
            => JsonFile.Write(this, Path.Combine(directoryPath, FileName));

        public static void Delete(string directoryPath)
            => File.Delete(Path.Combine(directoryPath, FileName));
    }
}
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Optimus
{
    public static class JsonFile
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
            {ContractResolver = new CamelCasePropertyNamesContractResolver()};
        
        public static T Read<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }
        
        public static void Write<T>(T obj, string path)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, JsonSettings);
            File.WriteAllText(path, json);
        }
    }
}
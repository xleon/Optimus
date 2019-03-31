using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Optimus
{
    public static class JsonFiles
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
            {ContractResolver = new CamelCasePropertyNamesContractResolver()};
        
        public static T ReadJson<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }
        
        public static void WriteJson<T>(T obj, string path)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, JsonSettings);
            File.WriteAllText(path, json);
        }
    }
}
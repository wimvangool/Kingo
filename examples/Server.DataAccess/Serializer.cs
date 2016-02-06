using System;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kingo.Samples.Chess
{
    public static class Serializer
    {
        private static readonly JsonSerializerSettings _SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = false
            },
            Formatting = Formatting.None,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.None
        };

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _SerializerSettings);
        }

        public static object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _SerializerSettings);
        }
    }
}

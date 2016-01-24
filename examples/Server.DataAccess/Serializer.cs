using System;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kingo.Samples.Chess
{
    internal static class Serializer
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

        internal static string Serialize(object aggregate)
        {
            return JsonConvert.SerializeObject(aggregate, _SerializerSettings);
        }

        internal static object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _SerializerSettings);
        }
    }
}

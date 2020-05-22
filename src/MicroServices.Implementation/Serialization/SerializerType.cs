using System;
using Kingo.MicroServices;

namespace Kingo.Serialization
{
    internal sealed class SerializerType : MicroProcessorComponent
    {
        private SerializerType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }

        public static bool IsSerializerType(Type type, out SerializerType serializer)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsSerializerType(component, out serializer);
            }
            serializer = null;
            return false;
        }

        private static bool IsSerializerType(MicroProcessorComponent component, out SerializerType serializer)
        {
            if (typeof(ISerializer).IsAssignableFrom(component.Type))
            {
                serializer = new SerializerType(component, typeof(ISerializer));
                return true;
            }
            serializer = null;
            return false;
        }
    }
}

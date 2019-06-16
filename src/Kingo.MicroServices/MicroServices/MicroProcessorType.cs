using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a <see cref="MicroProcessor"/> type.
    /// </summary>
    public sealed class MicroProcessorType : MicroProcessorComponent
    {
        internal MicroProcessorType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }        

        internal static bool IsMicroProcessorType(Type type, out MicroProcessorType processor)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsMicroProcessorType(component, out processor);
            }
            processor = null;
            return false;
        }

        private static bool IsMicroProcessorType(MicroProcessorComponent component, out MicroProcessorType processor)
        {
            if (typeof(MicroProcessor).IsAssignableFrom(component.Type))
            {
                processor = new MicroProcessorType(component, typeof(IMicroProcessor));
                return true;
            }
            processor = null;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a <see cref="MicroProcessor"/> type.
    /// </summary>
    public sealed class MicroProcessorType : MicroProcessorComponent
    {
        internal MicroProcessorType(MicroProcessorComponent component) :
            base(component) { }

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
                processor = new MicroProcessorType(component);
                return true;
            }
            processor = null;
            return false;
        }
    }
}

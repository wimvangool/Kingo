using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a type that implements one or more variations of the <see cref="IMessageIdFactory{TMessage}"/> interface.
    /// </summary>
    public sealed class MessageIdFactoryType : MicroProcessorComponent
    {
        private MessageIdFactoryType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }

        public static bool IsMessageIdFactory(MicroProcessorComponent component, out MessageIdFactoryType factoryType)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for components that implement one or more variations of the <see cref="IMessageIdFactory{TMessage}"/> interface.
    /// </summary>
    public abstract class MessageIdFactoryComponent : MicroProcessorComponent
    {
        private readonly MessageIdFactoryInterface[] _interfaces;

        internal MessageIdFactoryComponent(MessageIdFactoryComponent component) :
            this(component, component._interfaces) { }

        internal MessageIdFactoryComponent(MicroProcessorComponent component, params MessageIdFactoryInterface[] interfaces) :
            base(component, interfaces.Select(@interface => @interface.Type))
        {
            _interfaces = interfaces;
        }
    }
}

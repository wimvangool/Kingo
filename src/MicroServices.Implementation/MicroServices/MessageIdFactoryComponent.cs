using System.Linq;

namespace Kingo.MicroServices
{

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

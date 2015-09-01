using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    internal abstract class MessageHandlerDispatcher : IMessageHandler
    {
        public abstract IMessage Message
        {
            get;
        }

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            attribute = null;
            return false;
        }

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            attribute = null;
            return false;
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return Enumerable.Empty<TAttribute>();
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return Enumerable.Empty<TAttribute>();
        }

        public abstract Task InvokeAsync();        

        protected static void ThrowIfCancellationRequested()
        {
            MessageProcessor.CurrentMessage.ThrowIfCancellationRequested();
        }        
    }
}

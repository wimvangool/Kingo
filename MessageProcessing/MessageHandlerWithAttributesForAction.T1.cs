using System;
using System.Collections.Generic;
using System.Linq;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageHandlerWithAttributesForAction<TMessage> : MessageHandlerWithAttributes<TMessage> where TMessage : class
    {
        private readonly Action<TMessage> _action;

        public MessageHandlerWithAttributesForAction(Action<TMessage> action)
        {
            _action = action;
        }

        public override void Handle(TMessage message)
        {
            _action.Invoke(message);
        }

        public override IEnumerable<object> GetClassAttributesOfType(Type type, bool includeInherited = false)
        {
            return Enumerable.Empty<object>();
        }

        public override IEnumerable<object> GetMethodAttributesOfType(Type type, bool includeInherited = false)
        {
            return Enumerable.Empty<object>();
        }
    }
}

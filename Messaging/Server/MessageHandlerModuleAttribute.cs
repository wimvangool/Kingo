using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base-class for all <see cref="Attribute">Attributes</see> that are put on
    /// messages that are processed by a <see cref="IMessageProcessor" /> to instruct or configure
    /// certain modules in the pipeline of this processor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class MessageHandlerModuleAttribute : Attribute
    {
        #region [====== Message Attribute Cache ======]

        private static readonly ConcurrentDictionary<Type, MessageHandlerModuleAttribute[]> _MessageAttributeCache;

        static MessageHandlerModuleAttribute()
        {
            _MessageAttributeCache = new ConcurrentDictionary<Type, MessageHandlerModuleAttribute[]>();
        }

        internal static IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>(Type messageType) where TAttribute : MessageHandlerModuleAttribute
        {
            return from attribute in _MessageAttributeCache.GetOrAdd(messageType, GetDeclaredAttributesOn)
                   let targetAttribute = attribute as TAttribute
                   where targetAttribute != null
                   select targetAttribute;
        }

        private static MessageHandlerModuleAttribute[] GetDeclaredAttributesOn(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof(MessageHandlerModuleAttribute), true).Cast<MessageHandlerModuleAttribute>().ToArray();
        }

        #endregion
    }
}

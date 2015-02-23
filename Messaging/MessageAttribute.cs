using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Server;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Serves as a base-class for all <see cref="Attribute">Attributes</see> that are put on
    /// messages that are processed by a <see cref="IMessageProcessor" /> to instruct or configure
    /// certain modules in the pipeline of this processor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class MessageAttribute : Attribute
    {
        #region [====== Message Attribute Cache ======]

        private static readonly ConcurrentDictionary<Type, MessageAttribute[]> _MessageAttributeCache;

        static MessageAttribute()
        {
            _MessageAttributeCache = new ConcurrentDictionary<Type, MessageAttribute[]>();
        }

        internal static IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>(Type messageType) where TAttribute : MessageAttribute
        {
            return from attribute in _MessageAttributeCache.GetOrAdd(messageType, GetDeclaredAttributesOn)
                   let targetAttribute = attribute as TAttribute
                   where targetAttribute != null
                   select targetAttribute;
        }

        private static MessageAttribute[] GetDeclaredAttributesOn(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof(MessageAttribute), true).Cast<MessageAttribute>().ToArray();
        }

        #endregion
    }
}

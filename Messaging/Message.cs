using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    [Serializable]
    public abstract class Message : IMessage, IExtensibleDataObject
    {
        private ExtensionDataObject _extensionData;

        internal Message() { }

        internal Message(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _extensionData = message._extensionData;
        }

        #region [====== ExtensibleObject ======]

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExtensionDataObject IExtensibleDataObject.ExtensionData
        {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion

        #region [====== Copy ======]

        IMessage IMessage.Copy()
        {
            return CopyMessage();
        }

        internal abstract IMessage CopyMessage();

        #endregion

        #region [====== Validation ======]

        bool IMessage.TryGetValidationErrors(out ValidationErrorTree errorTree)
        {
            return TryGetValidationErrors(out errorTree);
        }

        internal abstract bool TryGetValidationErrors(out ValidationErrorTree errorTree);

        #endregion

        #region [====== Attributes ======]

        IEnumerable<TAttribute> IMessage.SelectAttributesOfType<TAttribute>()
        {
            return SelectAttributesOfType<TAttribute>();
        }

        internal virtual IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>() where TAttribute : Attribute
        {
            return AttributesOfType<TAttribute>(GetType());
        }        

        private static readonly ConcurrentDictionary<Type, Attribute[]> _MessageAttributeCache;

        static Message()
        {
            _MessageAttributeCache = new ConcurrentDictionary<Type, Attribute[]>();
        }

        internal static IEnumerable<TAttribute> AttributesOfType<TAttribute>(Type messageType) where TAttribute : Attribute
        {
            return from attribute in _MessageAttributeCache.GetOrAdd(messageType, GetDeclaredAttributesOn)
                   let targetAttribute = attribute as TAttribute
                   where targetAttribute != null
                   select targetAttribute;
        }

        private static Attribute[] GetDeclaredAttributesOn(Type messageType)
        {
            return messageType.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>().ToArray();
        }

        #endregion
    }
}

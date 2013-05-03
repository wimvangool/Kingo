using System;
using Microsoft.Practices.Unity;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// A LifetimeManager for Unity that registers it's dependencies at the current <see cref="MessageProcessorContext" />.
    /// </summary>
    public sealed class PerUnitOfWorkLifetimeManager : LifetimeManager
    {
        private readonly Guid _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerUnitOfWorkLifetimeManager" /> class.
        /// </summary>
        public PerUnitOfWorkLifetimeManager()
        {
            _key = Guid.NewGuid();
        }

        /// <inheritdoc />
        public override object GetValue()
        {
            var context = MessageProcessorContext.Current;            
            if (context != null)
            {
                object value;

                if (context.Cache.TryGetValue(_key, out value))
                {
                    return value;
                }
            }
            return null;
        }

        /// <inheritdoc />
        public override void RemoveValue()
        {
            var context = MessageProcessorContext.Current;
            if (context == null)
            {
                throw NewOutOfContextException();
            }
            context.Cache.Remove(_key);
        }

        /// <inheritdoc />
        public override void SetValue(object newValue)
        {
            var context = MessageProcessorContext.Current;
            if (context == null)
            {
                throw NewOutOfContextException();
            }
            context.Cache.Add(_key, newValue);
        }

        private static Exception NewOutOfContextException()
        {            
            return new InvalidOperationException(ExceptionMessages.PerUnitOfWorkLifetimeManager_OutOfContext);
        }
    }
}

using System;
using Microsoft.Practices.Unity;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// A LifetimeManager for Unity that registers it's dependencies at the current <see cref="UnitOfWorkContext" />.
    /// </summary>
    public sealed class PerUnitOfWorkLifetimeManager : LifetimeManager
    {
        private ICachedItem<object> _item;        

        /// <inheritdoc />
        public override object GetValue()
        {
            object value;

            if (_item != null && _item.TryGetValue(out value))
            {
                return value;
            }
            return null;
        }

        /// <inheritdoc />
        public override void RemoveValue()
        {
            _item = null;
        }

        /// <inheritdoc />
        public override void SetValue(object newValue)
        {
            var context = UnitOfWorkContext.Current;
            if (context == null)
            {
                throw NewOutOfContextException();
            }
            _item = context.Cache.Add(newValue, HandleValueRemoved);
        }

        private static void HandleValueRemoved(object value)
        {
            var disposable = value as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        private static Exception NewOutOfContextException()
        {            
            return new InvalidOperationException(ExceptionMessages.PerUnitOfWorkLifetimeManager_OutOfContext);
        }
    }
}

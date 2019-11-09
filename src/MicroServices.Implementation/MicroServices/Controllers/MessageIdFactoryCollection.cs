using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a collection of <see cref="MessageIdFactoryType" /> types.
    /// </summary>
    public sealed class MessageIdFactoryCollection : MicroProcessorComponentCollection
    {
        private readonly HashSet<MessageIdFactoryComponent> _instances;
        private readonly IServiceCollection _instanceCollection;

        internal MessageIdFactoryCollection()
        {
            _instances = new HashSet<MessageIdFactoryComponent>();
            _instanceCollection = new ServiceCollection();
        }

        #region [====== Add ======]

        /// <summary>
        /// Adds the specified <paramref name="factory" /> to this collection.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that the specified <paramref name="factory"/> can generate identifiers for.</typeparam>
        /// <param name="factory">The factory to add.</param>
        /// <returns><c>true</c> if the <paramref name="factory"/> was added; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance<TMessage>(Func<TMessage, string> factory) =>
            AddInstance(new MessageIdFactoryDelegate<TMessage>(factory));

        /// <summary>
        /// Adds the specified <paramref name="factory" /> to this collection.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that the specified <paramref name="factory"/> can generate identifiers for.</typeparam>
        /// <param name="factory">The factory to add.</param>
        /// <returns><c>true</c> if the <paramref name="factory"/> was added; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public bool AddInstance<TMessage>(IMessageIdFactory<TMessage> factory) =>
            Add(new MessageIdFactoryInstance<TMessage>(factory));

        private bool Add<TMessage>(MessageIdFactoryInstance<TMessage> factory)
        {
            _instances.Add(factory);
            _instanceCollection.AddTransient<IMessageIdFactory<TMessage>>(provider => factory);
            return true;
        }

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (MessageIdFactoryType.IsMessageIdFactory(component, out var factory))
            {
                return base.Add(factory);
            }
            return false;
        }

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        /// <inheritdoc />
        protected internal override IServiceCollection AddSpecificComponentsTo(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            foreach (var instance in _instanceCollection)
            {
                services.Add(instance);
            }
            return services.AddSingleton<IMessageIdFactory>(serviceProvider => new MessageIdFactory(serviceProvider));
        }

        #endregion
    }
}

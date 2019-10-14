using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a builder that can be used to configure
    /// the <see cref="IMicroProcessor" /> to use in your service.
    /// </summary>
    public interface IMicroProcessorBuilder : IMicroProcessorOptions
    {
        /// <summary>
        /// Represents a collection of <see cref="IMessageHandler{TMessage}"/> types that will be
        /// registered and used by the <see cref="IMicroProcessor" />.
        /// </summary>
        MessageHandlerCollection MessageHandlers
        {
            get;
        }

        /// <summary>
        /// Represents a collection of <see cref="IQuery{TResponse}"/> and <see cref="IQuery{TRequest, TResponse}"/> that will be
        /// registered and used by the <see cref="IMicroProcessor" />.
        /// </summary>
        QueryCollection Queries
        {
            get;
        }

        /// <summary>
        /// Represents a collection of <see cref="IMicroServiceBus"/> types that we be used to build a single
        /// <see cref="IMicroServiceBus"/> instance to be used by the <see cref="IMicroProcessor" />.
        /// </summary>
        MicroServiceBusControllerCollection MicroServiceBusControllers
        {
            get;
        }

        /// <summary>
        /// Represents a collection of <see cref="IMessageIdFactory{TMessage}" /> types that will be
        /// registered and used by the <see cref="IMicroProcessor" />.
        /// </summary>
        MessageIdFactoryCollection MessageIdFactory
        {
            get;
        }

        /// <summary>
        /// Adds the specified <paramref name="components"/> to be registered and used by the <see cref="IMicroProcessor" />.
        /// </summary>
        /// <param name="components">The components to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="components"/> is <c>null</c>.
        /// </exception>
        void Add(IMicroProcessorComponentCollection components);
    }
}

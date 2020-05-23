using System;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented by a class, represents a builder that can be used to configure
    /// the <see cref="IMicroProcessor" /> to use in your service.
    /// </summary>
    public interface IMicroProcessorBuilder
    {
        /// <summary>
        /// Configures the general settings of the <see cref="IMicroProcessor" /> to use.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the settings.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        IMicroProcessorBuilder ConfigureSettings(Action<MicroProcessorSettings> configurator);

        /// <summary>
        /// Configures all data-contracts that the <see cref="IMicroProcessor" /> will recognize and be able
        /// to serialize and deserialize with a <see cref="IDataContractSerializer"/>.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the collection.</param>
        /// <returns>This builder.</returns>
        IMicroProcessorBuilder ConfigureDataContracts(Action<DataContractCollection> configurator = null) =>
            ConfigureComponents(configurator);

        /// <summary>
        /// Configures all serializers that the <see cref="IMicroProcessor" /> can use to serialize and deserialize
        /// messages and domain data.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the collection.</param>
        /// <returns>This builder.</returns>
        IMicroProcessorBuilder ConfigureSerializers(Action<SerializerCollection> configurator = null) =>
            ConfigureComponents(configurator);

        /// <summary>
        /// Configures the message-pipeline that will be used by the <see cref="IMicroProcessor"/>
        /// to consume and/or produce different types of messages.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the message factory.</param>
        /// <returns>This builder.</returns>
        IMicroProcessorBuilder ConfigureMessagePipeline(Action<MessagePipeline> configurator = null) =>
            ConfigureComponents(configurator);

        /// <summary>
        /// Configures all <see cref="IMessageHandler{TMessage}"/>-types to be used by the
        /// <see cref="IMicroProcessor"/>.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the collection.</param>
        /// <returns>This builder.</returns>
        IMicroProcessorBuilder ConfigureMessageHandlers(Action<MessageHandlerCollection> configurator = null) =>
            ConfigureComponents(configurator);

        /// <summary>
        /// Configures all <see cref="IQuery{TResponse}"/>- and <see cref="IQuery{TRequest,TResponse}"/>-types to be
        /// used by the <see cref="IMicroProcessor"/>.
        /// </summary>
        /// <param name="configurator">Delegate that will be used to configure the collection.</param>
        /// <returns>This builder.</returns>
        IMicroProcessorBuilder ConfigureQueries(Action<QueryCollection> configurator = null) =>
            ConfigureComponents(configurator);

        /// <summary>
        /// Configures a specific collection of components that are to be registered for use by the <see cref="IMicroProcessor" />.
        /// </summary>
        /// <typeparam name="TCollection">Type of the collection to configure.</typeparam>
        /// <param name="configurator">Delegate that will be used to configure the message factory.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configurator"/> is <c>null</c>.
        /// </exception>
        IMicroProcessorBuilder ConfigureComponents<TCollection>(Action<TCollection> configurator = null) where TCollection : IMicroProcessorComponentCollection, new();
    }
}

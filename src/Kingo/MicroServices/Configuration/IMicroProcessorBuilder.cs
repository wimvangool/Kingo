namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented by a class, represents a builder that can be used to configure
    /// the <see cref="IMicroProcessor" /> to use in your service.
    /// </summary>
    public interface IMicroProcessorBuilder
    {
        /// <summary>
        /// Can be used to add one or more service-bus instances that will be used by the processor.
        /// </summary>
        MicroServiceBusBuilder ServiceBus
        {
            get;
        }

        /// <summary>
        /// Can be used to configure which message-handlers will be used by the processor.
        /// </summary>
        MessageHandlerFactoryBuilder MessageHandlers
        {
            get;
        }

        /// <summary>
        /// Can be used to configure the pipeline of the processor.
        /// </summary>
        MicroProcessorPipelineFactoryBuilder Pipeline
        {
            get;
        }
    }
}

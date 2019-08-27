namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a builder that can be used to configure
    /// the <see cref="IMicroProcessor" /> to use in your service.
    /// </summary>
    public interface IMicroProcessorBuilder : IMicroProcessorOptions
    {
        /// <summary>
        /// Can be used to configure which message-handlers will be used by the processor.
        /// </summary>
        MicroProcessorComponentCollection Components
        {
            get;
        }                     
    }
}

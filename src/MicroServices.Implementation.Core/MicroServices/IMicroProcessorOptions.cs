namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a set of options that can be set to configure the behavior of a <see cref="MicroProcessor" />.
    /// </summary>
    public interface IMicroProcessorOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="UnitOfWorkMode" />.
        /// </summary>
        UnitOfWorkMode UnitOfWorkMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the options for the endpoints.
        /// </summary>
        MicroProcessorEndpointOptions Endpoints
        {
            get;
        }
    }
}

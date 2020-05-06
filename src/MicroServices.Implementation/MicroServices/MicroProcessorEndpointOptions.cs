using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of options that can be set for the endpoints of a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorEndpointOptions
    {        
        internal MicroProcessorEndpointOptions()
        {
            ServiceName = "DefaultService";
        }

        private MicroProcessorEndpointOptions(MicroProcessorEndpointOptions options)
        {
            ServiceName = options.ServiceName;
        }

        internal MicroProcessorEndpointOptions Copy() =>
            new MicroProcessorEndpointOptions(this);

        #region [====== ServiceName ======]

        private string _serviceName;

        /// <summary>
        /// Gets or sets the name of the (micro)service.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public string ServiceName
        {
            get => _serviceName;
            set => _serviceName = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion
    }
}

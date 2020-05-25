namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a set of options that can be used to configure the run-time behavior
    /// of a <see cref="MicroServiceBusController" />.
    /// </summary>
    public class MicroServiceBusControllerOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusControllerOptions" /> class.
        /// </summary>
        public MicroServiceBusControllerOptions()
        {
            Modes = MicroServiceBusModes.SendingAndReceiving;
        }

        /// <summary>
        /// Gets or sets the <see cref="MicroServiceBusModes" /> of the bus.
        /// </summary>
        public MicroServiceBusModes Modes
        {
            get;
            set;
        }
    }
}

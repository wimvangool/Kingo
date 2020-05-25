using System.Collections.Generic;
using Kingo.MicroServices.Controllers;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of <see cref="MicroServiceBusController"/>-types that are to used by a
    /// <see cref="IMicroProcessor"/> to send and/or receive messages.
    /// </summary>
    public sealed class MicroServiceBusControllerCollection : MicroProcessorComponentCollection
    {
        private readonly HashSet<MicroServiceBusControllerType> _controllers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusControllerCollection" /> class.
        /// </summary>
        public MicroServiceBusControllerCollection()
        {
            _controllers = new HashSet<MicroServiceBusControllerType>();
        }

        /// <inheritdoc />
        public override IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            _controllers.GetEnumerator();

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (MicroServiceBusControllerType.IsMicroServiceBusController(component, out var controller))
            {
                _controllers.Add(controller);
                return true;
            }
            return false;
        }
    }
}

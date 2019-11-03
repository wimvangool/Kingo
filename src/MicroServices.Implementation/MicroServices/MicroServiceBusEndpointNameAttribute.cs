using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to a message or message handler, defines the name of this message type
    /// or message handler type that is to be used in the name of the service bus endpoint,
    /// if the name-format includes the name of a message or message handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MicroServiceBusEndpointNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusEndpointNameAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the message or message handler.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public MicroServiceBusEndpointNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// The name of the message or message handler.
        /// </summary>
        public string Name
        {
            get;
        }
    }
}

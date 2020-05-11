using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of options that can be used to configure what kinds of messages a <see cref="IMicroProcessor" />
    /// should validate.
    /// </summary>
    [Flags]
    public enum MessageValidationOptions
    {
        /// <summary>
        /// Indicates no message are validated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the processor will not accept messages that were resolved to <see cref="MessageKind.Undefined" />.
        /// If this flag is not set, the processor will automatically convert these kinds of messages to the kind that is
        /// expected in that context, and then validate them if required by the other options.
        /// </summary>
        BlockUndefined = 1 << 0,

        /// <summary>
        /// Indicates messages of kind <see cref="MessageKind.Command"/> should be validated.
        /// </summary>
        Commands = 1 << 1,

        /// <summary>
        /// Indicates messages of kind <see cref="MessageKind.Event"/> should be validated.
        /// </summary>
        Events = 1 << 2,

        /// <summary>
        /// Indicates messages of kind <see cref="MessageKind.Request"/> should be validated.
        /// </summary>
        Requests = 1 << 3,

        /// <summary>
        /// Indicates messages of kind <see cref="MessageKind.Response"/> should be validated.
        /// </summary>
        Responses = 1 << 4,

        /// <summary>
        /// Indicates that all messages should be validated.
        /// </summary>
        AllMessageKinds = Commands | Events | Requests | Responses,

        /// <summary>
        /// Indicates that all messages should be validated while also blocking undefined messages.
        /// </summary>
        All = BlockUndefined | AllMessageKinds
    }
}

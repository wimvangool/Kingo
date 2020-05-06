using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of options to configure a <see cref="IMessageFactory" /> on how to deal with message-validation.
    /// </summary>
    [Flags]
    public enum MessageValidationOptions
    {
        None = 0,

        /// <summary>
        /// Indicates that input-messages should be validated.
        /// </summary>
        Input = 1 << 0,

        /// <summary>
        /// Indicates that output-messages should be validated.
        /// </summary>
        Output = 1 << 1,

        /// <summary>
        /// Indicates that the processor will accept messages of <see cref="MessageKind.Undefined" /> and
        /// convert them automatically to the expected kind, depending on the context of the operation.
        /// </summary>
        UndefinedMessages = 1 << 2,

        /// <summary>
        /// Indicates that messages of <see cref="MessageKind.Command" /> should be validated.
        /// </summary>
        Commands = 1 << 3,

        /// <summary>
        /// Indicates that messages of <see cref="MessageKind.Event" /> should be validated.
        /// </summary>
        Events = 1 << 4,

        /// <summary>
        /// Indicates that messages of <see cref="MessageKind.Request" /> should be validated.
        /// </summary>
        Requests = 1 << 5,

        /// <summary>
        /// Indicates that messages of <see cref="MessageKind.Response" /> should be validated.
        /// </summary>
        Responses = 1 << 6,

        /// <summary>
        /// Represents the default configuration for a processor.
        /// </summary>
        Default = Input | Commands | Requests
    }
}

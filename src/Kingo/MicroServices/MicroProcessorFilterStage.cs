using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific stage in the processing pipeline of a <see cref="MicroProcessor" />.
    /// </summary>
    public enum MicroProcessorFilterStage
    {
        /// <summary>
        /// Represents the first stage, where any exceptions are caught and handled as desired.
        /// </summary>
        ExceptionHandlingStage,

        /// <summary>
        /// Represents the second stage, where the principal is authorized for the current operation.
        /// </summary>
        AuthorizationStage,

        /// <summary>
        /// Represents the third stage, where the input-message, if any, is validated.
        /// </summary>
        ValidationStage,

        /// <summary>
        /// Represents the fourth stage, where any arbitrary operations can be performed before
        /// or after the actual message handler or query is invoked.
        /// </summary>
        ProcessingStage
    }
}

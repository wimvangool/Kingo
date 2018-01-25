namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a specific stage within the pipeline of a <see cref="IMicroProcessor" />.
    /// </summary>
    public enum MicroProcessorPipelineStage
    {
        /// <summary>
        /// Represents the stage where exceptions are being handled and associated activity, such as logging, takes place.
        /// </summary>
        ExceptionHandlingStage,

        /// <summary>
        /// Represents the stage where the request is being authorized.
        /// </summary>
        AuthorizationStage,

        /// <summary>
        /// Represents the stage where the message is being validated.
        /// </summary>
        ValidationStage,

        /// <summary>
        /// Represents the stage where the message is being processed, right before it is dispatched to the handler or query.
        /// </summary>
        ProcessingStage
    }
}

namespace Kingo.MicroServices
{
    /// <summary>
    /// Indicates which direction the message is coming from or moving in with regards to the message-flow inside a processor.
    /// </summary>
    public enum MessageDirection
    {
        /// <summary>
        /// Indicates the message was directly fed to the processor. This typically means the message
        /// was received through a user-interface, an API or a service-bus.
        /// </summary>
        Input,

        /// <summary>
        /// Indicates the message was created and is being processed inside the processor.
        /// </summary>
        Internal,

        /// <summary>
        /// Indicates the message was created inside the processor and is meant to be sent out through a service-bus
        /// or returned as a result of the operation.
        /// </summary>
        Output
    }
}

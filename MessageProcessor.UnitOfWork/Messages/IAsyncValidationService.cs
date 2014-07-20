using System;

namespace YellowFlare.MessageProcessing.Messages
{
    /// <summary>
    /// Represents a service that is used to validate a certain property of a message and requires
    /// remote information to do so. As such, this service will retrieve this information asynchronously
    /// and raise the <see cref="ValidationRequired" /> event when it is ready to validate.
    /// </summary>
    public interface IAsyncValidationService
    {
        /// <summary>
        /// Occurs when this service has enough information to perform the validation.
        /// </summary>
        event EventHandler ValidationRequired;
    }
}

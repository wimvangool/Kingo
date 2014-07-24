using System.ComponentModel;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a message that supports change tracking and self-validation.
    /// </summary>    
    public interface IMessage : IHasChangesIndicator, IIsValidIndicator, IDataErrorInfo
    {
        /// <summary>
        /// Indicates whether or not this message has any changes.
        /// </summary>
        new bool HasChanges
        {
            get;
            set;
        }

        /// <summary>
        /// Validates all values of this message and then updates the validation-state.
        /// </summary>
        void Validate();
    }
}

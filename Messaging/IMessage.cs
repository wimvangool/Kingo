namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a message that supports change tracking and self-validation.
    /// </summary>    
    public interface IMessage : IHasChangesIndicator, IIsValidIndicator, IDataErrorInfo
    {
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// A copy of this message, including the validation-state. If <paramref name="makeReadOnly"/> is <c>true</c>,
        /// all data properties of the copy will be readonly. In addition, the returned copy will be marked unchanged,
        /// even if this message is marked as changed. If the copy is readonly, the HasChanges-flag cannot be
        /// set to <c>true</c>.
        /// </returns>
        IMessage Copy(bool makeReadOnly);

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

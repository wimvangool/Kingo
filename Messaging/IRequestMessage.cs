namespace System.ComponentModel
{
    /// <summary>
    /// Represents a request-message that supports change-tracking and validation.
    /// </summary>
    public interface IRequestMessage : INotifyHasChanges, INotifyIsValid, IDataErrorInfo        
    {
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// A copy of this message. If <paramref name="makeReadOnly"/> is <c>true</c>,
        /// all data properties of the copy will be readonly. In addition, the returned copy will be marked unchanged,
        /// even if this message is marked as changed. If the copy is readonly, the HasChanges-flag cannot be
        /// set to <c>true</c>.
        /// </returns>
        IRequestMessage Copy(bool makeReadOnly); 

        /// <summary>
        /// Marks this message as unchanged.
        /// </summary>
        void AcceptChanges();

        /// <summary>
        /// Validates all values of this message and then updates the validation-state.
        /// </summary>
        void Validate();

        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>        
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.</returns>
        bool IsNotValid(out MessageErrorTree errorTree);
    }
}

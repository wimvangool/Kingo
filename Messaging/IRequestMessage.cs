namespace System.ComponentModel
{
    /// <summary>
    /// Represents a request-message that supports change-tracking and validation.
    /// </summary>
    public interface IRequestMessage : IMessage, INotifyHasChanges, INotifyIsValid, IDataErrorInfo, IEditableObject
    {        
        /// <summary>
        /// Creates and returns a new <see cref="RequestMessageEditScope" /> that can be used to commit or roll back
        /// any changes that were made inside this scope.
        /// </summary>                
        /// <returns>A new <see cref="RequestMessageEditScope" />.</returns>
        RequestMessageEditScope CreateEditScope();

        /// <summary>
        /// Creates and returns a new <see cref="RequestMessageEditScope" /> that can be used to commit or roll back
        /// any changes that were made inside this scope.
        /// </summary>                
        /// <param name="state">
        /// Optional state that can be provided to correlate any changes to this message to a specific scope.
        /// </param>
        /// <returns>A new <see cref="RequestMessageEditScope" />.</returns>
        RequestMessageEditScope CreateEditScope(object state);

        /// <summary>
        /// Creates and returns a new <see cref="RequestMessageEditScope" /> that can be used to commit or roll back
        /// any changes that were made inside this scope.
        /// </summary>        
        /// <param name="suppressValidation">
        /// Indicates whether or not automatic validation of this message should be suppressed while the scope is active.
        /// </param>        
        /// <returns>A new <see cref="RequestMessageEditScope" />.</returns>
        RequestMessageEditScope CreateEditScope(bool suppressValidation);

        /// <summary>
        /// Creates and returns a new <see cref="RequestMessageEditScope" /> that can be used to commit or roll back
        /// any changes that were made inside this scope.
        /// </summary>        
        /// <param name="suppressValidation">
        /// Indicates whether or not automatic validation of this message should be suppressed while the scope is active.
        /// </param>
        /// <param name="state">
        /// Optional state that can be provided to correlate any changes to this message to a specific scope.
        /// </param>
        /// <returns>A new <see cref="RequestMessageEditScope" />.</returns>
        RequestMessageEditScope CreateEditScope(bool suppressValidation, object state);

        /// <summary>
        /// Marks this message as unchanged.
        /// </summary>
        void AcceptChanges();        

        /// <summary>
        /// Validates all values of this message and then updates the validation-state.
        /// </summary>
        void Validate();

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
    }
}

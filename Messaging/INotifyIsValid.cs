namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a component that can indicate whether or not it is valid.
    /// </summary>
    public interface INotifyIsValid : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when <see cref="IsValid" /> changed.
        /// </summary>
        event EventHandler IsValidChanged;

        /// <summary>
        /// Indicates whether or not the component is valid.
        /// </summary>
        bool IsValid
        {
            get;
        }
    }
}

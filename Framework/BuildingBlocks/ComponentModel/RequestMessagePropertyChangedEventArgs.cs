using System.ComponentModel;

namespace Kingo.BuildingBlocks.ComponentModel
{
    /// <summary>
    /// Provides some additional data for the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
    /// </summary>
    public class RequestMessagePropertyChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// The old value of the property.
        /// </summary>
        public readonly object OldValue;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public readonly object NewValue;

        /// <summary>
        /// The state was provided when an <see cref="RequestMessageViewModelEditScope" /> was created.
        /// </summary>
        public readonly object State;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMessagePropertyChangedEventArgs" /> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="state">The state was provided when an <see cref="RequestMessageViewModelEditScope" /> was created.</param>
        public RequestMessagePropertyChangedEventArgs(string propertyName, object oldValue, object newValue, object state) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
            State = state;
        }
    }
}

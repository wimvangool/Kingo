using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a set of <see cref="ControlModeInputBinding">Settings</see> that define which inputs are bound to specific camera actions.
    /// </summary>
    [ContentProperty(nameof(InputBindings))]
    public sealed class ControlMode : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMode" /> class.
        /// </summary>
        public ControlMode()
        {
            InputBindings = new ObservableCollection<ControlModeInputBinding>();
            InputBindings.CollectionChanged += HandleInputBindingsChanged;
        }        

        #region [====== Settings ======]

        /// <summary>
        /// Gets the collection of <see cref="ControlModeInputBinding">Settings</see> that have been defined for this mode.
        /// </summary>
        public ObservableCollection<ControlModeInputBinding> InputBindings
        {
            get;
        }

        private void HandleInputBindingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a set of <see cref="IControlModeCommandBinding">CommandBindings</see> that define which inputs are bound to specific camera actions.
    /// </summary>
    [ContentProperty(nameof(CommandBindings))]
    public sealed class ControlMode : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlMode" /> class.
        /// </summary>
        public ControlMode()
        {
            CommandBindings = new ObservableCollection<IControlModeCommandBinding>();
            CommandBindings.CollectionChanged += HandleInputBindingsChanged;
        }

        #region [====== Key ======]

        /// <summary>
        /// Backing-field of the <see cref="Key"/>-property.
        /// </summary>
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register(nameof(Key), typeof(object), typeof(ControlMode), new FrameworkPropertyMetadata(HandleKeyChanged));

        /// <summary>
        /// Gets or sets the key of this control-mode.
        /// </summary>
        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Occurs when the <see cref="Key"/>-property has changed.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs<object>> KeyChanged;

        private static void HandleKeyChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var controlMode = instance as ControlMode;
            if (controlMode != null)
            {
                controlMode.OnKeyChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnKeyChanged(object oldValue, object newValue)
        {
            KeyChanged.Raise(this, new PropertyChangedEventArgs<object>(oldValue, newValue));
        }

        #endregion

        #region [====== CommandBindings ======]

        /// <summary>
        /// Gets the collection of <see cref="IControlModeCommandBinding">CommandBindings</see> that have been defined for this mode.
        /// </summary>
        public ObservableCollection<IControlModeCommandBinding> CommandBindings
        {
            get;
        }

        private void HandleInputBindingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsActivated)
            {
                Deactivate(e.OldItems);
                Activate(e.NewItems);                
            }
        }

        private void Activate(IEnumerable items)
        {            
            foreach (var inputBinding in Cast(items))
            {
                inputBinding.Activate(this);
            }
        }

        private static void Deactivate(IEnumerable items)
        {
            foreach (var inputBinding in Cast(items))
            {
                inputBinding.Deactivate();
            }
        }

        private static IEnumerable<IControlModeCommandBinding> Cast(IEnumerable items)
        {
            return items == null ? Enumerable.Empty<IControlModeCommandBinding>() : items.Cast<IControlModeCommandBinding>();
        }

        #endregion

        #region [====== InputSource ======]

        /// <summary>
        /// Backing-field of the <see cref="InputSource"/>-property.
        /// </summary>
        public static readonly DependencyProperty InputSourceProperty =
            DependencyProperty.Register(nameof(InputSource), typeof(UIElement), typeof(ControlMode));

        /// <summary>
        /// Gets the <see cref="UIElement" /> that serves as the input-source for the controller.
        /// </summary>
        public UIElement InputSource
        {
            get { return (UIElement) GetValue(InputSourceProperty); }
            private set { SetValue(InputSourceProperty, value); }
        }        

        #endregion

        #region [====== Controller ======]

        /// <summary>
        /// Backing-field of the <see cref="Controller"/>-property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IProjectionCameraController), typeof(ControlMode));

        /// <summary>
        /// Gets the controller that effectively controls the camera.
        /// </summary>
        public IProjectionCameraController Controller
        {
            get { return (IProjectionCameraController) GetValue(ControllerProperty); }
            private set { SetValue(ControllerProperty, value); }
        }

        #endregion

        #region [====== IsActivated ======]

        /// <summary>
        /// Backing-field of the <see cref="IsActivated"/>-property.
        /// </summary>
        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register(nameof(IsActivated), typeof(bool), typeof(ControlMode));

        /// <summary>
        /// Indicates whether or not this control-mode is currently active.
        /// </summary>
        public bool IsActivated
        {
            get { return (bool) GetValue(IsActivatedProperty); }
            private set { SetValue(IsActivatedProperty, value); }
        }

        #endregion

        #region [====== Activate & Deactivate ======]

        internal void Activate(UIElement inputSource, IProjectionCameraController controller)
        {
            InputSource = inputSource;
            Controller = controller;

            foreach (var inputBinding in CommandBindings)
            {
                inputBinding.Activate(this);
            }

            IsActivated = true;
        }

        internal void Deactivate()
        {
            ClearValue(IsActivatedProperty);

            foreach (var inputBinding in CommandBindings)
            {
                inputBinding.Deactivate();
            }
            
            ClearValue(ControllerProperty);
            ClearValue(InputSourceProperty);
        }

        #endregion
    }
}

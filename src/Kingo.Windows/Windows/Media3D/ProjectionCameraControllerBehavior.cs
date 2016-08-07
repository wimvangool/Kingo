using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using Kingo.Resources;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When attached as a behavior to a <see cref="ProjectionCamera"/>, adds camera navigation such
    /// as moving, panning, zooming and orbiting the camera.
    /// </summary>
    [ContentProperty(nameof(ControlModes))]
    public sealed class ProjectionCameraControllerBehavior : Behavior<ProjectionCamera>
    {
        #region [====== State ======]

        private abstract class State
        {
            protected abstract ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            protected void MoveTo(State newState)
            {
                Behavior._currentState.Exit();
                Behavior._currentState = newState;
                Behavior._currentState.Enter();
            }

            protected virtual void Enter() { }

            protected virtual void Exit() { }

            public virtual void OnAttached()
            {
                throw NewInvalidOperationException();
            }

            public virtual void OnDetaching()
            {
                throw NewInvalidOperationException();
            }

            public virtual void OnInputSourceChanged() { }

            public virtual void OnControllerChanged(IProjectionCameraController oldController, IProjectionCameraController newController) { }

            public virtual void OnActiveModeKeyChanged(object newActiveKeyMode) { }

            public virtual void OnControlModeAdded(ControlMode controlMode) { }

            public virtual void OnControlModeRemoved(ControlMode controlMode) { }

            private InvalidOperationException NewInvalidOperationException([CallerMemberName] string operationName = null)
            {
                var messageFormat = ExceptionMessages.State_InvalidOperation;
                var message = string.Format(messageFormat, operationName, GetType().Name);
                return new InvalidOperationException(message);
            }
        }

        #endregion

        #region [====== DetachedState ======]

        private sealed class DetachedState : State
        {            
            public DetachedState(ProjectionCameraControllerBehavior behavior)
            {
                Behavior = behavior;
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }            

            public override void OnAttached()
            {
                MoveTo(new PassiveState(Behavior));
            }
        }

        #endregion

        #region [====== AttachedState ======]

        private abstract class AttachedState : State
        {
            public override void OnDetaching()
            {
                MoveTo(new DetachedState(Behavior));
            }            
        }

        #endregion

        #region [====== PassiveState ======]

        private sealed class PassiveState : AttachedState
        {
            public PassiveState(ProjectionCameraControllerBehavior behavior)
            {
                Behavior = behavior;
            }

            private bool CanMoveToActiveState
            {
                get { return Behavior.Controller != null; }
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            protected override void Enter()
            {
                foreach (var controlMode in Behavior.ControlModes)
                {
                    controlMode.KeyChanged += HandleControlModeKeyChanged;
                }
                TryMoveToActiveState();
            }

            protected override void Exit()
            {
                foreach (var controlMode in Behavior.ControlModes)
                {
                    controlMode.KeyChanged -= HandleControlModeKeyChanged;
                }
            }

            private void HandleControlModeKeyChanged(object sender, PropertyChangedEventArgs<object> e)
            {
                if (CanMoveToActiveState)
                {
                    var controlMode = sender as ControlMode;
                    if (controlMode != null && Equals(controlMode.Key, Behavior.ActiveModeKey))
                    {
                        MoveTo(new ActiveState(Behavior, controlMode));
                    }
                }
            }

            public override void OnActiveModeKeyChanged(object newActiveKeyMode)
            {
                TryMoveToActiveState(newActiveKeyMode);
            }

            public override void OnControlModeAdded(ControlMode controlMode)
            {
                controlMode.KeyChanged += HandleControlModeKeyChanged;

                if (CanMoveToActiveState && Equals(controlMode.Key, Behavior.ActiveModeKey))
                {
                    MoveTo(new ActiveState(Behavior, controlMode));
                }
            }

            public override void OnControlModeRemoved(ControlMode controlMode)
            {
                controlMode.KeyChanged -= HandleControlModeKeyChanged;
            }

            private void TryMoveToActiveState()
            {
                TryMoveToActiveState(Behavior.ActiveModeKey);
            }

            private void TryMoveToActiveState(object activeModeKey)
            {
                ControlMode newActiveMode;

                if (TrySelectNewActiveControlMode(activeModeKey, out newActiveMode))
                {
                    MoveTo(new ActiveState(Behavior, newActiveMode));
                }
            }

            private bool TrySelectNewActiveControlMode(object activeModeKey, out ControlMode newActiveMode)
            {
                var controller = Behavior.Controller;
                if (controller != null)
                {
                    newActiveMode = Behavior.ControlModes.FirstOrDefault(controlMode => Equals(controlMode.Key, activeModeKey));
                    return newActiveMode != null;
                }
                newActiveMode = null;
                return false;
            }

        }

        #endregion

        #region [====== ActiveState ======]

        private sealed class ActiveState : AttachedState
        {
            private readonly ControlMode _activeMode;

            public ActiveState(ProjectionCameraControllerBehavior behavior, ControlMode activeMode)
            {
                Behavior = behavior;

                _activeMode = activeMode;
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            protected override void Enter()
            {                
                Enter(Behavior.Controller);
            }

            private void Enter(IProjectionCameraController controller)
            {
                AttachCamera(controller, Behavior.AssociatedObject);

                _activeMode.KeyChanged += HandleControlModeKeyChanged;
                _activeMode.Activate(Behavior.InputSource, Behavior.Controller);
            }

            protected override void Exit()
            {
                Exit(Behavior.Controller);
            }

            private void Exit(IProjectionCameraController controller)
            {
                _activeMode.KeyChanged -= HandleControlModeKeyChanged;
                _activeMode.Deactivate();

                DetachCamera(controller);
            }

            public override void OnDetaching()
            {
                MoveTo(new DetachedState(Behavior));
            }

            public override void OnInputSourceChanged()
            {
                Exit();
                Enter();
            }

            public override void OnControllerChanged(IProjectionCameraController oldController, IProjectionCameraController newController)
            {
                if (newController == null)
                {
                    DetachCamera(oldController);

                    MoveTo(new PassiveState(Behavior));
                }
                else
                {
                    Exit(oldController);
                    Enter(newController);
                }
            }

            private void HandleControlModeKeyChanged(object sender, PropertyChangedEventArgs<object> e)
            {
                MoveTo(new PassiveState(Behavior));
            }

            public override void OnActiveModeKeyChanged(object newActiveKeyMode)
            {
                MoveTo(new PassiveState(Behavior));
            }

            public override void OnControlModeRemoved(ControlMode controlMode)
            {
                if (ReferenceEquals(_activeMode, controlMode))
                {
                    MoveTo(new PassiveState(Behavior));
                }
            }

            private static void AttachCamera(IProjectionCameraController controller, ProjectionCamera camera)
            {
                controller.Camera = camera;
            }

            private static void DetachCamera(IProjectionCameraController controller)
            {
                if (controller != null)
                {
                    controller.Camera = null;
                }
            }
        }

        #endregion

        private readonly ObservableCollection<ControlMode> _controlModes;
        private State _currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraControllerBehavior" /> class.
        /// </summary>
        public ProjectionCameraControllerBehavior()
            : this(new ProjectionCameraController()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionCameraControllerBehavior" /> class.
        /// </summary>
        /// <param name="controller">The controller that is used to control the associated <see cref="ProjectionCamera" />.</param>
        public ProjectionCameraControllerBehavior(IProjectionCameraController controller)
        {
            _currentState = new DetachedState(this);
            _controlModes = new ObservableCollection<ControlMode>();
            _controlModes.CollectionChanged += HandleControlModesChanged;

            Controller = controller;            
        }        

        #region [====== InputSource ======]

        /// <summary>
        /// Backing-field of the <see cref="InputSource"/>-property.
        /// </summary>
        public static readonly DependencyProperty InputSourceProperty =
            DependencyProperty.Register(nameof(InputSource), typeof(UIElement), typeof(ProjectionCameraControllerBehavior), new FrameworkPropertyMetadata(HandleInputSourceChanged));

        /// <summary>
        /// Gets or sets the <see cref="UIElement" /> that is used to attach all eventhandlers that control the navigation to..
        /// </summary>
        public UIElement InputSource
        {
            get { return (UIElement)GetValue(InputSourceProperty); }
            set { SetValue(InputSourceProperty, value); }
        }

        private static void HandleInputSourceChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var behavior = instance as ProjectionCameraControllerBehavior;
            if (behavior != null)
            {
                behavior._currentState.OnInputSourceChanged();
            }
        }


        #endregion

        #region [====== Controller ======]

        /// <summary>
        /// Backing-field of the <see cref="Controller"/>-property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register(nameof(Controller), typeof(IProjectionCameraController), typeof(ProjectionCameraControllerBehavior), new FrameworkPropertyMetadata(HandleControllerChanged));

        /// <summary>
        /// Gets or sets the controller that is used to control the associated <see cref="ProjectionCamera" />.
        /// </summary>
        public IProjectionCameraController Controller
        {
            get { return (IProjectionCameraController) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        private static void HandleControllerChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var behavior = instance as ProjectionCameraControllerBehavior;
            if (behavior != null)
            {
                behavior._currentState.OnControllerChanged(e.OldValue as IProjectionCameraController, e.NewValue as IProjectionCameraController);
            }
        }

        #endregion

        #region [====== ActiveModeKey ======]

        /// <summary>
        /// Backing-field of the <see cref="ActiveModeKey"/>-property.
        /// </summary>
        public static readonly DependencyProperty ActiveModeKeyProperty =
            DependencyProperty.Register(nameof(ActiveModeKey), typeof(object), typeof(ProjectionCameraControllerBehavior), new FrameworkPropertyMetadata(HandleActiveModeKeyChanged));

        /// <summary>
        /// Gets or sets the key of the mode that should be activated.
        /// </summary>
        public object ActiveModeKey
        {
            get { return GetValue(ActiveModeKeyProperty); }
            set { SetValue(ActiveModeKeyProperty, value); }
        }

        private static void HandleActiveModeKeyChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var behavior = instance as ProjectionCameraControllerBehavior;
            if (behavior != null)
            {
                behavior._currentState.OnActiveModeKeyChanged(e.NewValue);
            }
        }

        #endregion    

        #region [====== ControlModes ======]

        /// <summary>
        /// Returns the collection of <see cref="ControlMode">ControlModes</see> that have been defined on this behavior.
        /// </summary>
        public Collection<ControlMode> ControlModes
        {
            get { return _controlModes; }
        }

        private void HandleControlModesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var controlMode in e.OldItems.Cast<ControlMode>())
                {
                    _currentState.OnControlModeRemoved(controlMode);
                }
            }
            if (e.NewItems != null)
            {
                foreach (var controlMode in e.NewItems.Cast<ControlMode>())
                {
                    _currentState.OnControlModeAdded(controlMode);
                }
            }
        }        

        #endregion

        #region [====== Attaching and Detaching ======]

        /// <inheritdoc />
        protected override void OnAttached()
        {
            base.OnAttached();

            _currentState.OnAttached();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {           
            _currentState.OnDetaching();

            base.OnDetaching();
        }

        #endregion
    }
}

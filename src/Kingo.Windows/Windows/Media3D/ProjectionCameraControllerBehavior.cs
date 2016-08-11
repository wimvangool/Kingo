using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Kingo.Resources;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When attached as a behavior to a <see cref="ProjectionCamera"/>, adds camera navigation such
    /// as moving, panning, zooming and orbiting the camera.
    /// </summary>
    [ContentProperty(nameof(ControlModes))]
    public sealed class ProjectionCameraControllerBehavior : Behavior<Viewport3D>
    {
        #region [====== State ======]

        private abstract class State
        {
            protected Viewport3D Viewport => Behavior.AssociatedObject;

            protected abstract ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            protected void MoveTo(State newState)
            {                
                Behavior.CurrentState = newState;                                                            
            }

            internal virtual void Enter()
            {
                Debug.WriteLine("Entered State: {0}.", GetType().Name as object);
            }

            internal virtual void Exit() { }

            public virtual void OnAttached()
            {
                throw NewInvalidOperationException();
            }

            public virtual void OnDetaching()
            {
                throw NewInvalidOperationException();
            }           

            public virtual void OnControllerChanged(IProjectionCameraController newController) { }

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
                MoveTo(new LoadingState(Behavior));
            }            
        }

        #endregion

        #region [====== LoadingState ======]

        private sealed class LoadingState : State
        {
            private bool _moveToAttachedState = true;

            public LoadingState(ProjectionCameraControllerBehavior behavior)
            {
                Behavior = behavior;
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            internal override void Enter()
            {
                base.Enter();

                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action) OnLoaded);
            }

            internal override void Exit()
            {
                _moveToAttachedState = false;

                base.Exit();
            }

            private void OnLoaded()
            {
                if (_moveToAttachedState)
                {
                    if (Behavior._focusManager.HasFocus(Viewport))
                    {
                        MoveTo(new PassiveState(Behavior));
                    }
                    else
                    {
                        MoveTo(new UnfocusedState(Behavior, true));
                    }
                }
            }            

            public override void OnDetaching()
            {
                MoveTo(new DetachedState(Behavior));
            }           
        }

        #endregion

        #region [====== AttachedState ======]

        private abstract class AttachedState : State
        {
            private FocusWatcher _focusWatcher;

            internal override void Enter()
            {
                base.Enter();

                _focusWatcher = Behavior._focusManager.CreateFocusWatcher(Viewport);
                _focusWatcher.GotFocus += HandleGotFocus;
                _focusWatcher.LostFocus += HandleLostFocus;
            }            

            internal override void Exit()
            {
                _focusWatcher.LostFocus -= HandleLostFocus;
                _focusWatcher.GotFocus -= HandleGotFocus;
                _focusWatcher = null;               

                base.Exit();
            }                       

            protected virtual void HandleGotFocus(object sender, EventArgs e) { }
            
            protected virtual void HandleLostFocus(object sender, EventArgs e) { }

            public override void OnDetaching()
            {
                MoveTo(new DetachedState(Behavior));
            }                        
        }

        #endregion

        #region [====== UnfocusedState ======]

        private sealed class UnfocusedState : AttachedState
        {            
            public UnfocusedState(ProjectionCameraControllerBehavior behavior, bool isFirstTime = false)
            {
                Behavior = behavior;
                IsFirstTime = isFirstTime;
            }            

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            private bool IsFirstTime
            {
                get;
            }

            internal override void Enter()
            {         
                base.Enter();

                Viewport.MouseDown += HandleViewportMouseDown;  
                
                if (IsFirstTime)
                {
                    Focus(Viewport);
                }
            }            

            internal override void Exit()
            {
                Viewport.MouseDown -= HandleViewportMouseDown;

                base.Exit();
            }

            private void HandleViewportMouseDown(object sender, MouseButtonEventArgs e)
            {
                Focus(Viewport);
            }

            private void Focus(UIElement viewport)
            {
                Behavior._focusManager.Focus(viewport);
            }

            protected override void HandleGotFocus(object sender, EventArgs e)
            {
                MoveTo(new PassiveState(Behavior));
            }                       
        }

        #endregion

        #region [====== FocusedState ======]

        private abstract class FocusedState : AttachedState
        {                     
            protected override void HandleLostFocus(object sender, EventArgs e)
            {
                MoveTo(new UnfocusedState(Behavior));
            }                        
        }

        #endregion

        #region [====== PassiveState ======]

        private sealed class PassiveState : FocusedState
        {
            public PassiveState(ProjectionCameraControllerBehavior behavior)
            {
                Behavior = behavior;
            }

            private bool HasController
            {
                get { return Behavior.Controller != null; }
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            internal override void Enter()
            {                                
                base.Enter();

                foreach (var controlMode in Behavior.ControlModes)
                {
                    controlMode.KeyChanged += HandleControlModeKeyChanged;
                }
                TryMoveToActiveState();
            }

            internal override void Exit()
            {
                foreach (var controlMode in Behavior.ControlModes)
                {
                    controlMode.KeyChanged -= HandleControlModeKeyChanged;
                }
                base.Exit();
            }

            private void HandleControlModeKeyChanged(object sender, PropertyChangedEventArgs<object> e)
            {
                if (HasController)
                {
                    var controlMode = sender as ControlMode;
                    if (controlMode != null && Equals(controlMode.Key, Behavior.ActiveModeKey))
                    {
                        MoveTo(new ActiveState(Behavior, controlMode, Behavior.Controller));
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

                if (HasController && Equals(controlMode.Key, Behavior.ActiveModeKey))
                {
                    MoveTo(new ActiveState(Behavior, controlMode, Behavior.Controller));
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
                    MoveTo(new ActiveState(Behavior, newActiveMode, Behavior.Controller));
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

        private sealed class ActiveState : FocusedState
        {            
            public ActiveState(ProjectionCameraControllerBehavior behavior, ControlMode activeMode, IProjectionCameraController controller)
            {
                Behavior = behavior;
                ActiveMode = activeMode;
                Controller = controller;
            }

            protected override ProjectionCameraControllerBehavior Behavior
            {
                get;
            }

            private ControlMode ActiveMode
            {
                get;
            }

            private IProjectionCameraController Controller
            {
                get;
            }

            internal override void Enter()
            {
                base.Enter();

                CameraPropertyDescriptor.AddValueChanged(Behavior, HandleCameraChanged);
                Controller.Camera = Viewport.Camera as ProjectionCamera;

                ActiveMode.KeyChanged += HandleControlModeKeyChanged;
                ActiveMode.Activate(Viewport, Controller);
            }

            internal override void Exit()
            {
                ActiveMode.KeyChanged -= HandleControlModeKeyChanged;
                ActiveMode.Deactivate();

                CameraPropertyDescriptor.RemoveValueChanged(Behavior, HandleCameraChanged);

                base.Exit();
            }            

            public override void OnDetaching()
            {
                MoveTo(new DetachedState(Behavior));
            }

            private void HandleCameraChanged(object sender, EventArgs e)
            {
                var controller = Behavior.Controller;
                if (controller != null)
                {
                    controller.Camera = Viewport.Camera as ProjectionCamera;
                }
            }

            private static DependencyPropertyDescriptor CameraPropertyDescriptor =>
                DependencyPropertyDescriptor.FromProperty(Viewport3D.CameraProperty, typeof(Viewport3D));

            public override void OnControllerChanged(IProjectionCameraController newController)
            {
                if (newController == null)
                {                    
                    MoveTo(new PassiveState(Behavior));
                }
                else
                {
                    MoveTo(new ActiveState(Behavior, ActiveMode, newController));
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
                if (ReferenceEquals(ActiveMode, controlMode))
                {
                    MoveTo(new PassiveState(Behavior));
                }
            }                                       
        }

        #endregion

        #region [====== FocusManager & Watcher ======]

        private sealed class FocusManager : IFocusManager
        {
            public bool HasFocus(UIElement element)
            {
                return ReferenceEquals(Keyboard.FocusedElement, element);
            }

            public void Focus(UIElement element)
            {
                element.Focusable = true;

                Keyboard.Focus(element);
            }

            public FocusWatcher CreateFocusWatcher(UIElement element)
            {
                return KeyboardFocusWatcher.StartWatching(element);
            }
        }

        private sealed class KeyboardFocusWatcher : FocusWatcher
        {
            private readonly UIElement _element;

            private KeyboardFocusWatcher(UIElement element)
            {
                _element = element;
            }

            private void StartWatching()
            {
                Keyboard.AddGotKeyboardFocusHandler(_element, HandleGotFocus);
                Keyboard.AddLostKeyboardFocusHandler(_element, HandleLostFocus);
            }

            private void StopWatching()
            {
                Keyboard.RemoveLostKeyboardFocusHandler(_element, HandleLostFocus);
                Keyboard.RemoveGotKeyboardFocusHandler(_element, HandleGotFocus);                
            }

            private void HandleGotFocus(object sender, KeyboardFocusChangedEventArgs e)
            {
                OnGotFocus();
            }

            private void HandleLostFocus(object sender, KeyboardFocusChangedEventArgs e)
            {
                OnLostFocus();
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                StopWatching();
            }

            public static KeyboardFocusWatcher StartWatching(UIElement element)
            {
                var watcher = new KeyboardFocusWatcher(element);                
                watcher.StartWatching();
                return watcher;
            }                        
        }

        #endregion

        private readonly ObservableCollection<ControlMode> _controlModes;
        private readonly IFocusManager _focusManager;      

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
            : this(controller, new FocusManager()) { }

        internal ProjectionCameraControllerBehavior(IProjectionCameraController controller, IFocusManager focusManager)
        {
            _currentState = new DetachedState(this);
            _focusManager = focusManager;

            _controlModes = new ObservableCollection<ControlMode>();
            _controlModes.CollectionChanged += HandleControlModesChanged;            

            Controller = controller;
        }

        #region [====== Current State ======]        

        /// <summary>
        /// Occurs when the internal state of the behavior changes.
        /// </summary>
        public event EventHandler StateChanged;

        private void OnStateChanged()
        {
            StateChanged.Raise(this);
        }

        private State _currentState;

        private State CurrentState
        {
            get { return _currentState; }
            set
            {
                if (_currentState != value)
                {
                    _currentState.Exit();
                    _currentState = value;
                    _currentState.Enter();

                    OnStateChanged();
                }
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
                behavior._currentState.OnControllerChanged(e.NewValue as IProjectionCameraController);
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

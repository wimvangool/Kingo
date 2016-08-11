using System;
using System.Runtime.CompilerServices;
using System.Windows;
using Kingo.Resources;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between a certain input like a Keyboard- or Mouse-event and a <see cref="ProjectionCameraController" />-command.
    /// </summary>
    public abstract class ControlModeInputBinding : DependencyObject
    {
        #region [====== State ======]

        private abstract class State
        {
            protected abstract ControlModeInputBinding InputBinding
            {
                get;
            }

            protected void MoveTo(State newState)
            {
                InputBinding._currentState.Exit();
                InputBinding._currentState = newState;
                InputBinding._currentState.Enter();
            }

            public virtual void Enter() { }

            public virtual void Exit() { }

            public virtual void Activate(ControlMode controlMode)
            {
                throw NewInvalidOperationException();
            }

            public virtual void Deactivate()
            {
                throw NewInvalidOperationException();
            }

            public virtual void OnCommandChanged(IProjectionCameraCommand newCommand) { }

            public virtual void OnCommandParameterChanged() { }

            public virtual void OnCommandTriggerRaised()
            {
                InputBinding.CommandTriggerRaised.Raise(InputBinding);
            }

            private InvalidOperationException NewInvalidOperationException([CallerMemberName] string operationName = null)
            {
                var messageFormat = ExceptionMessages.State_InvalidOperation;
                var message = string.Format(messageFormat, operationName, GetType().Name);
                return new InvalidOperationException(message);
            }
        }

        #endregion

        #region [====== DeactivatedState ======]

        private sealed class DeactivatedState : State
        {
            public DeactivatedState(ControlModeInputBinding inputBinding)
            {
                InputBinding = inputBinding;
            }

            protected override ControlModeInputBinding InputBinding
            {
                get;
            }

            public override void Activate(ControlMode controlMode)
            {
                var command = InputBinding.Command;
                if (command == null)
                {
                    MoveTo(new UnboundState(InputBinding, controlMode));
                }
                else
                {
                    MoveTo(new BoundState(InputBinding, controlMode, command));
                }
            }
        }

        #endregion

        #region [====== ActiveState ======]

        private abstract class ActiveState : State
        {
            protected ActiveState(ControlModeInputBinding inputBinding, ControlMode controlMode)
            {
                InputBinding = inputBinding;
                ControlMode = controlMode;
            }

            protected override ControlModeInputBinding InputBinding
            {
                get;
            }

            protected ControlMode ControlMode
            {
                get;
            }            

            public override void Enter() { }

            public override void Exit() { }

            public override void Deactivate()
            {
                MoveTo(new DeactivatedState(InputBinding));
            }
        }

        #endregion

        #region [====== UnboundState ======]

        private sealed class UnboundState : ActiveState
        {
            public UnboundState(ControlModeInputBinding inputBinding, ControlMode controlMode)
                : base(inputBinding, controlMode) { }

            public override void OnCommandChanged(IProjectionCameraCommand newCommand)
            {
                if (newCommand != null)
                {
                    MoveTo(new BoundState(InputBinding, ControlMode, newCommand));
                }
            }
        }

        #endregion

        #region [====== BoundState ======]

        private sealed class BoundState : ActiveState
        {
            private readonly IProjectionCameraCommand _command;
            private IProjectionCameraController _controller;
            private bool _skipCanExecuteCheck;

            public BoundState(ControlModeInputBinding inputBinding, ControlMode controlMode, IProjectionCameraCommand command)
                : base(inputBinding, controlMode)
            {
                _command = command;                
            }                        

            public override void Enter()
            {
                base.Enter();

                _command.CanExecuteChanged += HandleCommandCanExecuteChanged;
                _command.Add(_controller = ControlMode.Controller);

                InputBinding.ActivateBinding(ControlMode.InputSource);
            }

            public override void Exit()
            {
                InputBinding.DeactivateBinding(ControlMode.InputSource);

                _command.Remove(_controller);
                _command.CanExecuteChanged -= HandleCommandCanExecuteChanged;

                base.Exit();
            }

            public override void OnCommandChanged(IProjectionCameraCommand newCommand)
            {
                if (newCommand == null)
                {
                    MoveTo(new UnboundState(InputBinding, ControlMode));
                }
                else
                {
                    MoveTo(new BoundState(InputBinding, ControlMode, newCommand));
                }
            }

            public override void OnCommandTriggerRaised()
            {
                base.OnCommandTriggerRaised();

                object parameter;

                if (CanExecuteCommandWithCurrentParameter(out parameter))
                {
                    _command.Execute(parameter);
                }
            }

            public override void OnCommandParameterChanged()
            {
                _skipCanExecuteCheck = false;
            }

            private void HandleCommandCanExecuteChanged(object sender, EventArgs e)
            {
                _skipCanExecuteCheck = false;
            }

            private bool CanExecuteCommandWithCurrentParameter(out object parameter)
            {
                parameter = InputBinding.CommandParameter;

                if (_skipCanExecuteCheck || _command.CanExecute(parameter))
                {                                        
                    return _skipCanExecuteCheck = true;
                }                
                parameter = null;
                return false;
            }
        }

        #endregion

        private State _currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlModeInputBinding" /> class.
        /// </summary>
        protected ControlModeInputBinding()
        {
            _currentState = new DeactivatedState(this);
        }

        #region [====== Command ======]

        /// <summary>
        /// Backing-field of the <see cref="Command"/>-property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(IProjectionCameraCommand), typeof(ControlModeInputBinding), new FrameworkPropertyMetadata(HandleCommandPropertyChanged));

        /// <summary>
        /// Gets or sets the command that is used to control the camera.
        /// </summary>
        public IProjectionCameraCommand Command
        {
            get { return (IProjectionCameraCommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void HandleCommandPropertyChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var controlModeInputBinding = instance as ControlModeInputBinding;
            if (controlModeInputBinding != null)
            {
                controlModeInputBinding._currentState.OnCommandChanged(e.NewValue as IProjectionCameraCommand);
            }
        }        

        #endregion

        #region [====== CommandParameter ======]

        /// <summary>
        /// Backing-field of the <see cref="CommandParameter"/>-property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ControlModeInputBinding), new FrameworkPropertyMetadata(HandleCommandParameterPropertyChanged));

        /// <summary>
        /// Gets or sets the parameter used to execute the <see cref="Command" />.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private static void HandleCommandParameterPropertyChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var controlModeInputBinding = instance as ControlModeInputBinding;
            if (controlModeInputBinding != null)
            {
                controlModeInputBinding._currentState.OnCommandParameterChanged();
            }
        }

        #endregion

        #region [====== CommandTrigger ======]

        /// <summary>
        /// Occurs when the trigger that signals that the command should be executed, if possible, is raised.
        /// </summary>
        public event EventHandler CommandTriggerRaised;

        /// <summary>
        /// Raises the <see cref="CommandTriggerRaised"/> event and attempts to execute the associated
        /// <see cref="Command"/> with the current <see cref="CommandParameter"/>.
        /// </summary>
        protected virtual void OnCommandTriggerRaised()
        {
            _currentState.OnCommandTriggerRaised();
        }

        #endregion

        #region [====== Activate & Deactivate ======]        

        internal void Activate(ControlMode controlMode)
        {
            _currentState.Activate(controlMode);
        }

        /// <summary>
        /// Activates the input binding.
        /// </summary>
        protected abstract void ActivateBinding(UIElement inputSource);

        internal void Deactivate()
        {
            _currentState.Deactivate();
        }

        /// <summary>
        /// Deactivates the input binding.
        /// </summary>
        protected abstract void DeactivateBinding(UIElement inputSource);

        #endregion        
    }
}

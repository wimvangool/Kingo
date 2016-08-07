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

            public virtual void OnCommandChanged(IProjectionCameraControllerCommand newCommand) { }

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

            private ControlMode ControlMode
            {
                get;
            }

            public override void Enter()
            {
                InputBinding.ControlMode = ControlMode;
            }

            public override void Exit()
            {
                InputBinding.ControlMode = null;
            }

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

            public override void OnCommandChanged(IProjectionCameraControllerCommand newCommand)
            {
                if (newCommand != null)
                {
                    MoveTo(new BoundState(InputBinding, InputBinding.ControlMode, newCommand));
                }
            }
        }

        #endregion

        #region [====== BoundState ======]

        private sealed class BoundState : ActiveState
        {
            private bool _skipCanExecuteCheck;

            public BoundState(ControlModeInputBinding inputBinding, ControlMode controlMode, IProjectionCameraControllerCommand command)
                : base(inputBinding, controlMode)
            {
                Command = command;
            }            

            private IProjectionCameraControllerCommand Command
            {
                get;
            }

            public override void Enter()
            {
                base.Enter();

                Command.CanExecuteChanged += HandleCommandCanExecuteChanged;
                Command.Attach(InputBinding.ControlMode.Controller);

                InputBinding.ActivateBinding();
            }

            public override void Exit()
            {
                InputBinding.DeactivateBinding();

                Command.Detach();
                Command.CanExecuteChanged -= HandleCommandCanExecuteChanged;

                base.Exit();
            }

            public override void OnCommandChanged(IProjectionCameraControllerCommand newCommand)
            {
                if (newCommand == null)
                {
                    MoveTo(new UnboundState(InputBinding, InputBinding.ControlMode));
                }
                else
                {
                    MoveTo(new BoundState(InputBinding, InputBinding.ControlMode, newCommand));
                }
            }

            public override void OnCommandTriggerRaised()
            {
                base.OnCommandTriggerRaised();

                object parameter;

                if (CanExecuteCommandWithCurrentParameter(out parameter))
                {
                    Command.Execute(parameter);
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

                if (_skipCanExecuteCheck || Command.CanExecute(parameter))
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
            DependencyProperty.Register(nameof(Command), typeof(IProjectionCameraControllerCommand), typeof(ControlModeInputBinding), new FrameworkPropertyMetadata(HandleCommandPropertyChanged));

        /// <summary>
        /// Gets or sets the command that is used to control the camera.
        /// </summary>
        public IProjectionCameraControllerCommand Command
        {
            get { return (IProjectionCameraControllerCommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void HandleCommandPropertyChanged(DependencyObject instance, DependencyPropertyChangedEventArgs e)
        {
            var controlModeInputBinding = instance as ControlModeInputBinding;
            if (controlModeInputBinding != null)
            {
                controlModeInputBinding._currentState.OnCommandChanged(e.NewValue as IProjectionCameraControllerCommand);
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

        /// <summary>
        /// Gets the <see cref="ControlMode" /> this input-binding is part of.
        /// </summary>
        protected ControlMode ControlMode
        {
            get;
            private set;
        }

        internal void Activate(ControlMode controlMode)
        {
            _currentState.Activate(controlMode);
        }

        /// <summary>
        /// Activates the input binding.
        /// </summary>
        protected abstract void ActivateBinding();

        internal void Deactivate()
        {
            _currentState.Deactivate();
        }

        /// <summary>
        /// Deactivates the input binding.
        /// </summary>
        protected abstract void DeactivateBinding();

        #endregion        
    }
}

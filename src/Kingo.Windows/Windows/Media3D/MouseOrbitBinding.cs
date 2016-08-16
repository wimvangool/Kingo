using System.Windows;
using System.Windows.Input;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between the movement of the mouse and the orientation (yaw and pitch) of the camera.
    /// </summary>
    public sealed class MouseOrbitBinding : OrbitCommandBinding
    {
        #region [====== State ======]

        private abstract class State
        {
            public virtual void OnMouseDown(MouseEventArgs e) { }

            public virtual void OnMouseUp(MouseEventArgs e) { }

            protected FrameworkElement InputSource => Binding.ControlMode.InputSource;

            protected abstract MouseOrbitBinding Binding
            {
                get;
            }

            protected void MoveTo(State newState)
            {
                Binding._currentState.Exit();
                Binding._currentState = newState;
                Binding._currentState.Enter();
            }

            protected virtual void Enter() { }

            protected virtual void Exit() { }

            protected Point GetMousePosition(MouseEventArgs e)
            {
                return e.GetPosition(Binding.ControlMode.InputSource);
            }
        }

        #endregion

        #region [====== IdleState ======]

        private sealed class IdleState : State
        {            
            public IdleState(MouseOrbitBinding binding)
            {
                Binding = binding;
            }

            protected override MouseOrbitBinding Binding
            {
                get;
            }

            public override void OnMouseDown(MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    MoveTo(new OrbittingState(Binding, GetMousePosition(e)));
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    MoveTo(new RollingState(Binding, GetMousePosition(e)));
                }
            }
        }

        #endregion

        #region [====== OrbittingState ======]

        private sealed class OrbittingState : State
        {
            private Point _currentPosition;

            public OrbittingState(MouseOrbitBinding binding, Point currentPosition)
            {
                Binding = binding;

                _currentPosition = currentPosition;
            }

            protected override MouseOrbitBinding Binding
            {
                get;
            }

            public override void OnMouseUp(MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    MoveTo(new IdleState(Binding));
                }
            }

            protected override void Enter()
            {
                InputSource.CaptureMouse();
                InputSource.MouseMove += HandleMouseMove;
            }            

            protected override void Exit()
            {
                InputSource.MouseMove -= HandleMouseMove;
                InputSource.ReleaseMouseCapture();
            }

            private void HandleMouseMove(object sender, MouseEventArgs e)
            {
                _currentPosition = Binding.OrbitCamera(_currentPosition, GetMousePosition(e));
            }
        }

        #endregion

        #region [====== RollingState ======]

        private sealed class RollingState : State
        {
            private Point _currentPosition;

            public RollingState(MouseOrbitBinding binding, Point currentPosition)
            {
                Binding = binding;

                _currentPosition = currentPosition;
            }

            protected override MouseOrbitBinding Binding
            {
                get;
            }

            public override void OnMouseUp(MouseEventArgs e)
            {
                if (e.RightButton == MouseButtonState.Released)
                {
                    MoveTo(new IdleState(Binding));
                }
            }

            protected override void Enter()
            {
                InputSource.CaptureMouse();
                InputSource.MouseMove += HandleMouseMove;
            }

            protected override void Exit()
            {
                InputSource.MouseMove -= HandleMouseMove;
                InputSource.ReleaseMouseCapture();
            }

            private void HandleMouseMove(object sender, MouseEventArgs e)
            {
                _currentPosition = Binding.RollCamera(_currentPosition, GetMousePosition(e));
            }
        }

        #endregion

        private State _currentState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseOrbitBinding" /> class.
        /// </summary>
        public MouseOrbitBinding()
        {
            _currentState = new IdleState(this);
        }

        /// <inheritdoc />
        protected override void OnActivated()
        {
            ControlMode.InputSource.MouseDown += HandleMouseDown;
            ControlMode.InputSource.MouseUp += HandleMouseUp;
        }        

        /// <inheritdoc />
        protected override void OnDeactivating()
        {
            ControlMode.InputSource.MouseUp -= HandleMouseUp;
            ControlMode.InputSource.MouseDown -= HandleMouseDown;
        }

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            _currentState.OnMouseDown(e);
        }

        private void HandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            _currentState.OnMouseUp(e);
        }
    }
}

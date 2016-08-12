using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Represents a binding between certain keys and movement of the camera.
    /// </summary>
    public sealed class MoveBinding : ControlModeCommandBinding
    {        
        #region [====== Speed ======]

        /// <summary>
        /// Backing-field of the <see cref="Speed"/>-property.
        /// </summary>
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MoveBinding), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public double Speed
        {
            get { return (double) GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        #endregion

        #region [====== Left ======]

        /// <summary>
        /// Backing-field of the <see cref="Left"/>-property.
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera to the left.
        /// </summary>
        public Key Left
        {
            get { return (Key) GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        #endregion

        #region [====== Right ======]

        /// <summary>
        /// Backing-field of the <see cref="Right"/>-property.
        /// </summary>
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register(nameof(Right), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera to the right.
        /// </summary>
        public Key Right
        {
            get { return (Key) GetValue(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

        #endregion

        #region [====== Up ======]

        /// <summary>
        /// Backing-field of the <see cref="Up"/>-property.
        /// </summary>
        public static readonly DependencyProperty UpProperty =
            DependencyProperty.Register(nameof(Up), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera up-wards.
        /// </summary>
        public Key Up
        {
            get { return (Key) GetValue(UpProperty); }
            set { SetValue(UpProperty, value); }
        }

        #endregion

        #region [====== Down ======]

        /// <summary>
        /// Backing-field of the <see cref="Down"/>-property.
        /// </summary>
        public static readonly DependencyProperty DownProperty =
            DependencyProperty.Register(nameof(Down), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera down-wards.
        /// </summary>
        public Key Down
        {
            get { return (Key) GetValue(DownProperty); }
            set { SetValue(DownProperty, value); }
        }

        #endregion

        #region [====== Forward ======]

        /// <summary>
        /// Backing-field of the <see cref="Forward"/>-property.
        /// </summary>
        public static readonly DependencyProperty ForwardProperty =
            DependencyProperty.Register(nameof(Forward), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera forwards.
        /// </summary>
        public Key Forward
        {
            get { return (Key) GetValue(ForwardProperty); }
            set { SetValue(ForwardProperty, value); }
        }

        #endregion

        #region [====== Backward ======]

        /// <summary>
        /// Backing-field of the <see cref="Backward"/>-property.
        /// </summary>
        public static readonly DependencyProperty BackwardProperty =
            DependencyProperty.Register(nameof(Backward), typeof(Key), typeof(MoveBinding));

        /// <summary>
        /// Gets or sets the key that moves the camera backwards.
        /// </summary>
        public Key Backward
        {
            get { return (Key) GetValue(BackwardProperty); }
            set { SetValue(BackwardProperty, value); }
        }

        #endregion

        #region [====== Activate & Deactivate ======]                   

        /// <inheritdoc />
        protected override void OnActivated()
        {
            CompositionTarget.Rendering += HandleNextFrame;
        }

        /// <inheritdoc />
        protected override void OnDeactivating()
        {
            CompositionTarget.Rendering -= HandleNextFrame;
        }

        private void HandleNextFrame(object sender, EventArgs e)
        {
            Vector3D moveDirection;

            if (ControlMode.Controller.CanMove && TryGetNextMove(out moveDirection))
            {
                ControlMode.Controller.Move(moveDirection);
            }            
        }

        private bool TryGetNextMove(out Vector3D moveDirection)
        {
            moveDirection = new Vector3D();

            if (Keyboard.Modifiers == ModifierKeys.None && AddAllMovesTo(ref moveDirection))
            {
                moveDirection.Normalize();
                moveDirection *= Speed;
                return true;
            }
            return false;
        }

        private bool AddAllMovesTo(ref Vector3D moveDirection)
        {
            return
                AddLeftMoveTo(ref moveDirection) |
                AddRightMoveTo(ref moveDirection) |
                AddUpMoveTo(ref moveDirection) |
                AddDownMoveTo(ref moveDirection) |
                AddForwardMoveTo(ref moveDirection) |
                AddBackwardMoveTo(ref moveDirection);
        }

        private bool AddLeftMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Left, ControlMode.Controller.Left, ref moveDirection);
        }

        private bool AddRightMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Right, ControlMode.Controller.Right, ref moveDirection);
        }

        private bool AddUpMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Up, ControlMode.Controller.Up, ref moveDirection);
        }

        private bool AddDownMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Down, ControlMode.Controller.Down, ref moveDirection);
        }

        private bool AddForwardMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Forward, ControlMode.Controller.Forward, ref moveDirection);
        }

        private bool AddBackwardMoveTo(ref Vector3D moveDirection)
        {
            return AddMove(Backward, ControlMode.Controller.Backward, ref moveDirection);
        }

        private static bool AddMove(Key key, Vector3D move, ref Vector3D moveDirection)
        {
            if (key != Key.None && Keyboard.IsKeyDown(key))
            {
                moveDirection += move;
                return true;
            }
            return false;
        }                    

        #endregion
    }
}

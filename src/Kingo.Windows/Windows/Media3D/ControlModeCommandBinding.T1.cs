using System.Windows;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// Serves as a base-class for all types of input that can be bound to a specific camera-command.
    /// </summary>
    /// <typeparam name="TInput">Type of the input.</typeparam>
    public abstract class ControlModeCommandBinding<TInput> : ControlModeCommandBinding
    {
        #region [====== MoveSpeed ======]

        /// <summary>
        /// Backing-field of the <see cref="MoveSpeed"/>-property.
        /// </summary>
        public static readonly DependencyProperty MoveSpeedProperty =
            DependencyProperty.Register(nameof(MoveSpeed), typeof(double), typeof(ControlModeCommandBinding<TInput>), new PropertyMetadata(1.0, null, CoerceSpeed));

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public double MoveSpeed
        {
            get { return (double) GetValue(MoveSpeedProperty); }
            set { SetValue(MoveSpeedProperty, value); }
        }        

        #endregion

        #region [====== Left ======]

        /// <summary>
        /// Backing-field of the <see cref="Left"/>-property.
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera to the left.
        /// </summary>
        public TInput Left
        {
            get { return (TInput)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        #endregion

        #region [====== Right ======]

        /// <summary>
        /// Backing-field of the <see cref="Right"/>-property.
        /// </summary>
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register(nameof(Right), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera to the right.
        /// </summary>
        public TInput Right
        {
            get { return (TInput)GetValue(RightProperty); }
            set { SetValue(RightProperty, value); }
        }

        #endregion

        #region [====== Up ======]

        /// <summary>
        /// Backing-field of the <see cref="Up"/>-property.
        /// </summary>
        public static readonly DependencyProperty UpProperty =
            DependencyProperty.Register(nameof(Up), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera up-wards.
        /// </summary>
        public TInput Up
        {
            get { return (TInput)GetValue(UpProperty); }
            set { SetValue(UpProperty, value); }
        }

        #endregion

        #region [====== Down ======]

        /// <summary>
        /// Backing-field of the <see cref="Down"/>-property.
        /// </summary>
        public static readonly DependencyProperty DownProperty =
            DependencyProperty.Register(nameof(Down), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera down-wards.
        /// </summary>
        public TInput Down
        {
            get { return (TInput)GetValue(DownProperty); }
            set { SetValue(DownProperty, value); }
        }

        #endregion

        #region [====== Forward ======]

        /// <summary>
        /// Backing-field of the <see cref="Forward"/>-property.
        /// </summary>
        public static readonly DependencyProperty ForwardProperty =
            DependencyProperty.Register(nameof(Forward), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera forwards.
        /// </summary>
        public TInput Forward
        {
            get { return (TInput)GetValue(ForwardProperty); }
            set { SetValue(ForwardProperty, value); }
        }

        #endregion

        #region [====== Backward ======]

        /// <summary>
        /// Backing-field of the <see cref="Backward"/>-property.
        /// </summary>
        public static readonly DependencyProperty BackwardProperty =
            DependencyProperty.Register(nameof(Backward), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera backwards.
        /// </summary>
        public TInput Backward
        {
            get { return (TInput)GetValue(BackwardProperty); }
            set { SetValue(BackwardProperty, value); }
        }

        #endregion

        #region [====== AxisLeft ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisLeft"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisLeftProperty =
            DependencyProperty.Register(nameof(AxisLeft), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing left.
        /// </summary>
        public TInput AxisLeft
        {
            get { return (TInput) GetValue(AxisLeftProperty); }
            set { SetValue(AxisLeftProperty, value); }
        }

        #endregion

        #region [====== AxisRight ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisRight"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisRightProperty =
            DependencyProperty.Register(nameof(AxisRight), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing right.
        /// </summary>
        public TInput AxisRight
        {
            get { return (TInput) GetValue(AxisRightProperty); }
            set { SetValue(AxisRightProperty, value); }
        }

        #endregion

        #region [====== AxisUp ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisUp"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisUpProperty =
            DependencyProperty.Register(nameof(AxisUp), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing up.
        /// </summary>
        public TInput AxisUp
        {
            get { return (TInput) GetValue(AxisUpProperty); }
            set { SetValue(AxisUpProperty, value); }
        }

        #endregion

        #region [====== AxisDown ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisDown"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisDownProperty =
            DependencyProperty.Register(nameof(AxisDown), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing down.
        /// </summary>
        public TInput AxisDown
        {
            get { return (TInput) GetValue(AxisDownProperty); }
            set { SetValue(AxisDownProperty, value); }
        }

        #endregion

        #region [====== AxisForward ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisForward"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisForwardProperty =
            DependencyProperty.Register(nameof(AxisForward), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing forward.
        /// </summary>
        public TInput AxisForward
        {
            get { return (TInput) GetValue(AxisForwardProperty); }
            set { SetValue(AxisForwardProperty, value); }
        }

        #endregion

        #region [====== AxisBackward ======]

        /// <summary>
        /// Backing-field of the <see cref="AxisBackward"/>-property.
        /// </summary>
        public static readonly DependencyProperty AxisBackwardProperty =
            DependencyProperty.Register(nameof(AxisBackward), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that moves the camera along the world-axis that is currently pointing backward.
        /// </summary>
        public TInput AxisBackward
        {
            get { return (TInput) GetValue(AxisBackwardProperty); }
            set { SetValue(AxisBackwardProperty, value); }
        }

        #endregion

        #region [====== ZoomSpeed ======]

        /// <summary>
        /// Backing-field of the <see cref="ZoomSpeed"/>-property.
        /// </summary>
        public static readonly DependencyProperty ZoomSpeedProperty =
            DependencyProperty.Register(nameof(ZoomSpeed), typeof(double), typeof(ControlModeCommandBinding<TInput>), new PropertyMetadata(1.0, null, CoerceSpeed));

        /// <summary>
        /// Gets or sets the speed at which the camera is zoomed in or out.
        /// </summary>
        public double ZoomSpeed
        {
            get { return (double) GetValue(ZoomSpeedProperty); }
            set { SetValue(ZoomSpeedProperty, value); }
        }

        #endregion

        #region [====== ZoomIn ======]

        /// <summary>
        /// Backing-field of the <see cref="ZoomIn"/>-property.
        /// </summary>
        public static readonly DependencyProperty ZoomInProperty =
            DependencyProperty.Register(nameof(ZoomIn), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that zooms in.
        /// </summary>
        public TInput ZoomIn
        {
            get { return (TInput) GetValue(ZoomInProperty); }
            set { SetValue(ZoomInProperty, value); }
        }

        #endregion

        #region [====== ZoomOut ======]

        /// <summary>
        /// Backing-field of the <see cref="ZoomOut"/>-property.
        /// </summary>
        public static readonly DependencyProperty ZoomOutProperty =
            DependencyProperty.Register(nameof(ZoomOut), typeof(TInput), typeof(ControlModeCommandBinding<TInput>));

        /// <summary>
        /// Gets or sets the input that zooms out.
        /// </summary>
        public TInput ZoomOut
        {
            get { return (TInput) GetValue(ZoomOutProperty); }
            set { SetValue(ZoomOutProperty, value); }
        }

        #endregion
    }
}

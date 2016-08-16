using System.Windows;
using System.Windows.Controls;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a binding that allows a user to orbit (yaw and pitch) the camera.
    /// </summary>
    public abstract class OrbitCommandBinding : ControlModeCommandBinding
    {
        #region [====== InvertYaw ======]

        /// <summary>
        /// Backing-field of the <see cref="InvertYaw"/>-property.
        /// </summary>
        public static readonly DependencyProperty InvertYawProperty =
            DependencyProperty.Register(nameof(InvertYaw), typeof(bool), typeof(OrbitCommandBinding));

        /// <summary>
        /// Indicates whether or not the yaw-rotation should be inverted.
        /// </summary>
        public bool InvertYaw
        {
            get { return (bool) GetValue(InvertYawProperty); }
            set { SetValue(InvertYawProperty, value); }
        }

        #endregion

        #region [====== InvertPitch ======]

        /// <summary>
        /// Backing-field of the <see cref="InvertPitch"/>-property.
        /// </summary>
        public static readonly DependencyProperty InvertPitchProperty =
            DependencyProperty.Register(nameof(InvertPitch), typeof(bool), typeof(OrbitCommandBinding));

        /// <summary>
        /// Indicates whether or not the pitch-rotation should be inverted.
        /// </summary>
        public bool InvertPitch
        {
            get { return (bool) GetValue(InvertPitchProperty); }
            set { SetValue(InvertPitchProperty, value); }
        }

        #endregion

        #region [====== InvertRoll ======]

        /// <summary>
        /// Backing-field of the <see cref="InvertRoll"/>-property.
        /// </summary>
        public static readonly DependencyProperty InvertRollProperty =
            DependencyProperty.Register(nameof(InvertRoll), typeof(bool), typeof(OrbitCommandBinding));

        /// <summary>
        /// Indicates whether or not the roll-rotation should be inverted.
        /// </summary>
        public bool InvertRoll
        {
            get { return (bool) GetValue(InvertRollProperty); }
            set { SetValue(InvertRollProperty, value); }
        }

        #endregion

        #region [====== YawRotationSpeed ======]

        /// <summary>
        /// Backing-field of the <see cref="YawRotationSpeed"/>-property.
        /// </summary>
        public static readonly DependencyProperty YawRotationSpeedProperty =
            DependencyProperty.Register(nameof(YawRotationSpeed), typeof(double), typeof(OrbitCommandBinding), new PropertyMetadata(1.0, null, CoerceSpeed));

        /// <summary>
        /// Gets or sets the speed at which the yaw-rotation are performed.
        /// </summary>
        public double YawRotationSpeed
        {
            get { return (double) GetValue(YawRotationSpeedProperty); }
            set { SetValue(YawRotationSpeedProperty, value); }
        }

        #endregion

        #region [====== PitchRotationSpeed ======]

        /// <summary>
        /// Backing-field of the <see cref="PitchRotationSpeed"/>-property.
        /// </summary>
        public static readonly DependencyProperty PitchRotationSpeedProperty =
            DependencyProperty.Register(nameof(PitchRotationSpeed), typeof(double), typeof(OrbitCommandBinding), new PropertyMetadata(1.0, null, CoerceSpeed));

        /// <summary>
        /// Gets or sets the sped at which the pitch-rotation is performed.
        /// </summary>
        public double PitchRotationSpeed
        {
            get { return (double) GetValue(PitchRotationSpeedProperty); }
            set { SetValue(PitchRotationSpeedProperty, value); }
        }

        #endregion

        #region [====== RollRotationSpeed ======]

        /// <summary>
        /// Backing-field of the <see cref="RollRotationSpeed"/>-property.
        /// </summary>
        public static readonly DependencyProperty RollRotationSpeedProperty =
            DependencyProperty.Register(nameof(RollRotationSpeed), typeof(double), typeof(OrbitCommandBinding), new PropertyMetadata(1.0, null, CoerceSpeed));

        /// <summary>
        /// Gets or sets the sped at which the roll-rotation is performed.
        /// </summary>
        public double RollRotationSpeed
        {
            get { return (double) GetValue(RollRotationSpeedProperty); }
            set { SetValue(RollRotationSpeedProperty, value); }
        }

        #endregion

        private const double _Zero = 0.0;

        /// <summary>
        /// Rotates the camera by yaw- and pitch-angles based on two points located on the <see cref="Viewport3D">viewport</see>.
        /// </summary>
        /// <param name="from">The point the input-device started.</param>
        /// <param name="to">The point the input-device ended.</param>
        protected virtual Point OrbitCamera(Point from, Point to)
        {
            var yawAngle = CalculateYawAngle(from.X, to.X, ControlMode.InputSource.ActualWidth);
            var pitchAngle = CalculatePitchAngle(from.Y, to.Y, ControlMode.InputSource.ActualHeight);

            ControlMode.Controller.YawPitchRoll(yawAngle, pitchAngle, _Zero);
            return to;
        }

        /// <summary>
        /// Rotates the camera by a roll-angle based on two points located on the <see cref="Viewport3D">viewport</see>.
        /// </summary>
        /// <param name="from">The point the input-device started.</param>
        /// <param name="to">The point the input-device ended.</param>
        /// <returns></returns>
        protected virtual Point RollCamera(Point from, Point to)
        {
            var isBottomHalf = from.Y > ControlMode.InputSource.ActualHeight / 2;
            var rollAngle = CalculateRollAngle(from.X, to.X, ControlMode.InputSource.ActualWidth, isBottomHalf);
            
            ControlMode.Controller.Roll(rollAngle);
            return to;
        }

        private double CalculateYawAngle(double from, double to, double width)
        {
            return YawRotationSpeed * RotationFactor(InvertYaw) * (to - from) / width;            
        }

        private double CalculatePitchAngle(double from, double to, double height)
        {
            return PitchRotationSpeed * RotationFactor(InvertPitch) * (to - from) / height;            
        }

        private double CalculateRollAngle(double from, double to, double width, bool isBottomHalf)
        {
            return RollRotationSpeed * RotationFactor(InvertRoll ^ isBottomHalf) * (to - from) / width;
        }

        private static double RotationFactor(bool invert)
        {
            return invert ? 90 : -90;
        }
    }
}

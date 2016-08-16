using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            DependencyProperty.Register(nameof(MoveSpeed), typeof(double), typeof(ControlModeCommandBinding<TInput>), new PropertyMetadata(1.0, null, CoerceMoveSpeed));

        /// <summary>
        /// Gets or sets the speed at which the camera moves.
        /// </summary>
        public double MoveSpeed
        {
            get { return (double) GetValue(MoveSpeedProperty); }
            set { SetValue(MoveSpeedProperty, value); }
        }

        private static object CoerceMoveSpeed(DependencyObject instance, object value)
        {
            return Math.Abs((double) value);
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
    }
}

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
    /// Represents a binding between the movement of the mouse and the orientation (yaw and pitch) of the camera.
    /// </summary>
    public sealed class MouseOrbitBinding : ControlModeCommandBinding
    {
        #region [====== InvertMouse ======]

        /// <summary>
        /// Backing-field of the <see cref="InvertMouse"/>-property.
        /// </summary>
        public static readonly DependencyProperty InvertMouseProperty =
            DependencyProperty.Register(nameof(InvertMouse), typeof(bool), typeof(MouseOrbitBinding));

        /// <summary>
        /// Indicates whether or not the mouse should be inverted.
        /// </summary>
        public bool InvertMouse
        {
            get { return (bool) GetValue(InvertMouseProperty); }
            set { SetValue(InvertMouseProperty, value); }
        }

        #endregion

        #region [====== Speed ======]

        /// <summary>
        /// Backing-field of the <see cref="Speed"/>-property.
        /// </summary>
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MouseOrbitBinding), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets or sets the speed at which the camera should be rotated.
        /// </summary>
        public double Speed
        {
            get { return (double) GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        #endregion

        #region [====== Binding Logic ======]

        /// <inheritdoc />
        protected override void OnActivated()
        {
            
        }

        /// <inheritdoc />
        protected override void OnDeactivating()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

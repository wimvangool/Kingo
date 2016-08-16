using System;
using System.Windows;
using Kingo.Resources;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a binding between a certain trigger or input and a camera-operation.
    /// </summary>
    public abstract class ControlModeCommandBinding : DependencyObject, IControlModeCommandBinding
    {
        #region [====== Activate & Deactivate ======]    

        /// <inheritdoc />
        public ControlMode ControlMode
        {
            get;
            private set;
        }    

        /// <inheritdoc />
        public void Activate(ControlMode controlMode)
        {
            if (ControlMode != null)
            {
                throw NewAlreadyActivatedException(this);
            }
            if (controlMode == null)
            {
                throw new ArgumentNullException(nameof(controlMode));
            }            
            ControlMode = controlMode;

            OnActivated();
        }

        /// <summary>
        /// This method is called when the input binding was activated.
        /// </summary>        
        protected abstract void OnActivated();

        /// <inheritdoc />
        public void Deactivate()
        {
            if (ControlMode == null)
            {
                throw NewAlreadyDeactivatedException(this);
            }
            OnDeactivating();

            ControlMode = null;
        }

        /// <summary>
        /// This method is called when the input binding is about to be deactivated.
        /// </summary>        
        protected abstract void OnDeactivating();

        internal static InvalidOperationException NewAlreadyActivatedException(object commandBinding)
        {
            var messageFormat = ExceptionMessages.ControlModeCommandBinding_AlreadyActivated;
            var message = string.Format(messageFormat, commandBinding.GetType().Name);
            return new InvalidOperationException(message);
        }

        internal static InvalidOperationException NewAlreadyDeactivatedException(object commandBinding)
        {
            var messageFormat = ExceptionMessages.ControlModeCommandBinding_AlreadyDeactivated;
            var message = string.Format(messageFormat, commandBinding.GetType().Name);
            return new InvalidOperationException(message);
        }

        #endregion
       
        internal static object CoerceSpeed(DependencyObject instance, object value)
        {
            return Math.Abs((double)value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kingo.Windows.Media3D
{
    /// <summary>
    /// When implemented by a derived class, represents a command that executes against a <see cref="IProjectionCameraController"/>.
    /// </summary>
    /// <typeparam name="TParameter">Parameter-type of this command.</typeparam>
    public abstract class CameraCommand<TParameter> : Command<TParameter>, IProjectionCameraCommand
    {
        private readonly List<IProjectionCameraController> _controllers;
        private List<IProjectionCameraController> _controllersThatCanExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraCommand{TParameter}" /> class.
        /// </summary>
        protected CameraCommand()
        {
            _controllers = new List<IProjectionCameraController>();
        }        

        /// <inheritdoc />
        public virtual void Add(IProjectionCameraController controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }
            _controllers.Add(controller);

            controller.PropertyChanged += HandleControllerPropertyChanged;

            OnCanExecuteChanged();
        }

        /// <inheritdoc />
        public virtual void Remove(IProjectionCameraController controller)
        {
            if (_controllers.Remove(controller) && _controllers.Count == 0)
            {
                controller.PropertyChanged -= HandleControllerPropertyChanged;

                OnCanExecuteChanged();
            }
        }

        private void HandleControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProjectionCameraController.Camera))
            {
                OnCanExecuteChanged();
            }
        }

        /// <inheritdoc />
        protected override void OnCanExecuteChanged()
        {
            _controllersThatCanExecute = null;

            base.OnCanExecuteChanged();
        }

        /// <inheritdoc />
        protected override bool CanExecuteCommand(TParameter parameter)
        {
            if (_controllersThatCanExecute == null)
            {
                var controllersThatCanExecute =
                    from controller in _controllers
                    where CanExecuteCommand(parameter, controller)
                    select controller;

                _controllersThatCanExecute = new List<IProjectionCameraController>(controllersThatCanExecute);
                _controllersThatCanExecute.TrimExcess();
            }
            return _controllersThatCanExecute.Count > 0;
        }

        /// <summary>
        /// When implemented, determines whether this command can be executed against the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="parameter">Parameter of the command.</param>
        /// <param name="controller">A controller.</param>
        /// <returns>
        /// <c>true</c> if the command can be executed; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool CanExecuteCommand(TParameter parameter, IProjectionCameraController controller);

        /// <inheritdoc />
        protected override void ExecuteCommand(TParameter parameter)
        {
            foreach (var controller in _controllersThatCanExecute)
            {
                ExecuteCommand(parameter, controller);
            }
        }

        /// <summary>
        /// When implemented, invokes one or more methods on the specified <paramref name="controller"/>
        /// using the specified <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">Parameter of the command.</param>
        /// <param name="controller">A controller.</param>
        protected abstract void ExecuteCommand(TParameter parameter, IProjectionCameraController controller);
    }
}

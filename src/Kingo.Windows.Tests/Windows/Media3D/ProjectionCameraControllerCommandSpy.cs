using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ProjectionCameraControllerCommandSpy : Command<object>, IProjectionCameraControllerCommand
    {
        private readonly List<object> _executions;
        private bool _canExecute;
        private int _attachCount;
        private int _detachCount;

        public ProjectionCameraControllerCommandSpy()
        {
            _executions = new List<object>();
        }

        protected override bool CanExecuteCommand(object parameter)
        {
            return _canExecute;
        }

        public new bool CanExecute
        {
            get { return _canExecute; }
            set
            {
                var oldValue = _canExecute;
                var newValue = value;

                if (oldValue != newValue)
                {
                    _canExecute = newValue;

                    OnCanExecuteChanged();
                }
            }
        }

        #region [====== Attach & Detach ======]

        public IProjectionCameraController Controller
        {
            get;
            private set;
        }

        public void Attach(IProjectionCameraController controller)
        {
            Controller = controller;

            _attachCount++;
        }

        public void Detach()
        {
            _detachCount++;
        }

        public void AssertAttachCountIs(int count)
        {
            Assert.AreEqual(count, _attachCount);
        }

        public void AssertDetachCountIs(int count)
        {
            Assert.AreEqual(count, _detachCount);
        }

        #endregion

        #region [====== Execution ======]

        protected override void ExecuteCommand(object parameter)
        {
            _executions.Add(parameter);
        }

        public void AssertExecutionCountIs(int count)
        {
            Assert.AreEqual(count, _executions.Count);
        }

        public void AssertExecutionParameterIs(int index, object parameter)
        {
            Assert.AreEqual(parameter, _executions[index]);
        }

        #endregion
    }
}

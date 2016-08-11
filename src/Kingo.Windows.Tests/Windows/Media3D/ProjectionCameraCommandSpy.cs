using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Windows.Media3D
{
    internal sealed class ProjectionCameraCommandSpy : CameraCommand<object>
    {
        private readonly List<object> _executions;
        private bool _canExecute;
        
        private int _addCount;
        private int _removeCount;

        public ProjectionCameraCommandSpy()
        {
            _executions = new List<object>();
        }

        protected override bool CanExecuteCommand(object parameter)
        {
            return _canExecute && base.CanExecuteCommand(parameter);
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

        #region [====== Add & Remove ======]      

        public override void Add(IProjectionCameraController controller)
        {
            base.Add(controller);

            _addCount++;
        }

        public override void Remove(IProjectionCameraController controller)
        {
            base.Remove(controller);

            _removeCount++;
        }

        public void AssertAddCountIs(int count)
        {
            Assert.AreEqual(count, _addCount);
        }

        public void AssertRemoveCountIs(int count)
        {
            Assert.AreEqual(count, _removeCount);
        }

        #endregion

        #region [====== Execution ======]

        protected override bool CanExecuteCommand(object parameter, IProjectionCameraController controller)
        {
            return true;
        }

        protected override void ExecuteCommand(object parameter, IProjectionCameraController controller)
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

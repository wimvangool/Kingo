using System;

namespace Kingo.MicroServices.Controllers
{
    internal abstract class MicroProcessorOperationTestResult : MicroProcessorOperationTestResultBase, IRunTestResult
    {
        #region [====== Verification ======]

        protected bool IsVerified
        {
            get;
            private set;
        }

        protected void OnVerified() =>
            IsVerified = true;

        public void Complete()
        {
            if (IsVerified)
            {
                return;
            }
            throw NewResultNotVerifiedException();
        }

        private static Exception NewResultNotVerifiedException() =>
            new TestFailedException(ExceptionMessages.MicroProcessorTestResult_ResultNotVerified);

        #endregion
    }
}

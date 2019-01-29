using System;

namespace Kingo.MicroServices
{
    internal abstract class MicroProcessorTestResult : MicroProcessorTestResultBase, IRunTestResult
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
            new MicroProcessorTestFailedException(ExceptionMessages.MicroProcessorTestResult_ResultNotVerified);

        #endregion
    }
}

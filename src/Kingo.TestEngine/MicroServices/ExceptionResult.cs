using System;

namespace Kingo.MicroServices
{
    internal abstract class ExceptionResult : IMicroProcessorTestResult
    {
        protected abstract MicroProcessorTestRunner TestRunner
        {
            get;
        }

        protected abstract Exception Exception
        {
            get;
        }

        public void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception
        {
            throw new NotImplementedException();
        }
    }
}

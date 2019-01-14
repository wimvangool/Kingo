using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal abstract class NoExceptionResult : IMicroProcessorTestResult
    {
        protected abstract MicroProcessorTestRunner TestRunner
        {
            get;
        }

        public void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryResult<TResponse> : IExecuteQueryResult<TResponse>
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroServices.ExceptionResult, IExecuteQueryResult<TResponse>
        {
            public ExceptionResult(MicroProcessorTestRunner testRunner, Exception exception)
            {
                TestRunner = testRunner;
                Exception = exception;
            }

            protected override MicroProcessorTestRunner TestRunner
            {
                get;
            }

            protected override Exception Exception
            {
                get;
            }

            public void IsExpectedResponse(Action<TResponse> assertion)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region [====== ResponseResult ======]

        private sealed class ResponseResult : NoExceptionResult, IExecuteQueryResult<TResponse>
        {
            public ResponseResult(MicroProcessorTestRunner testRunner, TResponse response)
            {
                TestRunner = testRunner;
                Response = response;
            }

            protected override MicroProcessorTestRunner TestRunner
            {
                get;
            }

            private TResponse Response
            {
                get;
            }

            public void IsExpectedResponse(Action<TResponse> assertion)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        private readonly IExecuteQueryResult<TResponse> _result;

        public ExecuteQueryResult(MicroProcessorTestRunner testRunner, Exception exception)
        {
            _result = new ExceptionResult(testRunner, exception);
        }

        public ExecuteQueryResult(MicroProcessorTestRunner testRunner, TResponse response)
        {
            _result = new ResponseResult(testRunner, response);
        }        

        public void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception =>
            _result.IsExpectedException(assertion);

        public void IsExpectedResponse(Action<TResponse> assertion) =>
            _result.IsExpectedResponse(assertion);
    }
}

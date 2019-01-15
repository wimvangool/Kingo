using System;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteQueryResult<TResponse> : MicroProcessorTestResult, IExecuteQueryResult<TResponse>
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorTestResult, IExecuteQueryResult<TResponse>
        {
            private readonly Exception _exception;

            public ExceptionResult(Exception exception)
            {                
                _exception = exception;
            }

            public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
                IsExpectedException(_exception, assertion);

            public void IsExpectedResponse(Action<TResponse> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== ResponseResult ======]

        private sealed class ResponseResult : MicroProcessorTestResult, IExecuteQueryResult<TResponse>
        {
            private readonly TResponse _response;

            public ResponseResult(TResponse response)
            {                
                _response = response;
            }

            public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsExpectedResponse(Action<TResponse> assertion)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                assertion.Invoke(_response);
            }            
        }

        #endregion

        private readonly IExecuteQueryResult<TResponse> _result;

        public ExecuteQueryResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public ExecuteQueryResult(TResponse response)
        {
            _result = new ResponseResult(response);
        }        

        public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
            _result.IsExpectedException(assertion);

        public void IsExpectedResponse(Action<TResponse> assertion) =>
            _result.IsExpectedResponse(assertion);
    }
}

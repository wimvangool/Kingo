using System;

namespace Kingo.MicroServices
{
    internal sealed class QueryResult<TResponse> : MicroProcessorTestResult, IQueryResult<TResponse>
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorTestResult, IQueryResult<TResponse>
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

        private sealed class ResponseResult : MicroProcessorTestResult, IQueryResult<TResponse>
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

        private readonly IQueryResult<TResponse> _result;

        public QueryResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public QueryResult(TResponse response)
        {
            _result = new ResponseResult(response);
        }        

        public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
            _result.IsExpectedException(assertion);

        public void IsExpectedResponse(Action<TResponse> assertion) =>
            _result.IsExpectedResponse(assertion);
    }
}

using System;

namespace Kingo.MicroServices
{
    internal sealed class QueryResult<TResponse> : MicroProcessorTestResult, IQueryResult<TResponse>
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorTestResultBase, IQueryResult<TResponse>
        {
            private readonly Exception _exception;

            public ExceptionResult(Exception exception)
            {                
                _exception = exception;
            }

            public override string ToString() =>
                _exception.GetType().FriendlyName();

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                IsExpectedException(_exception, assertion);

            public void IsResponse(Action<TResponse> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== ResponseResult ======]

        private sealed class ResponseResult : MicroProcessorTestResultBase, IQueryResult<TResponse>
        {
            private readonly TResponse _response;

            public ResponseResult(TResponse response)
            {                
                _response = response;
            }

            public override string ToString() =>
                typeof(TResponse).FriendlyName();

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsResponse(Action<TResponse> assertion)
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

        public override string ToString() =>
            _result.ToString();

        public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null)
        {
            try
            {
                return _result.IsExceptionOfType(assertion);
            }
            finally
            {
                OnVerified();
            }
        }            

        public void IsResponse(Action<TResponse> assertion)
        {
            try
            {
                _result.IsResponse(assertion);
            }
            finally
            {
                OnVerified();
            }
        }            
    }
}

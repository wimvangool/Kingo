using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryOperationTestResult<TResponse> : MicroProcessorOperationTestResult, IQueryOperationTestResult<TResponse>
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorOperationTestResultBase, IQueryOperationTestResult<TResponse>
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

            public void IsResponse(Action<MessageEnvelope<TResponse>> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== ResponseResult ======]

        private sealed class ResponseResult : MicroProcessorOperationTestResultBase, IQueryOperationTestResult<TResponse>
        {
            private readonly MessageEnvelope<TResponse> _output;

            public ResponseResult(MessageEnvelope<TResponse> output)
            {                
                _output = output;
            }

            public override string ToString() =>
                typeof(TResponse).FriendlyName();

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsResponse(Action<MessageEnvelope<TResponse>> assertion) =>
                NotNull(assertion).Invoke(_output);

            private static Action<MessageEnvelope<TResponse>> NotNull(Action<MessageEnvelope<TResponse>> assertion) =>
                assertion ?? throw new ArgumentNullException(nameof(assertion));
        }

        #endregion

        private readonly IQueryOperationTestResult<TResponse> _result;

        public QueryOperationTestResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public QueryOperationTestResult(IQueryOperationResult<TResponse> result)
        {
            _result = new ResponseResult(result.Output);
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

        public void IsResponse(Action<MessageEnvelope<TResponse>> assertion)
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

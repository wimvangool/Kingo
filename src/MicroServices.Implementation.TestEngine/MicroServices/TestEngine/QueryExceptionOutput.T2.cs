using System;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class QueryExceptionOutput<TRequest, TResponse> : IQueryTestOutput<TRequest, TResponse>
    {
        #region [====== IsExceptionMethod<TException> ======]

        private sealed class IsExceptionMethod<TException> : ITestOutputAssertMethod where TException : MicroProcessorOperationException
        {
            private readonly QueryExceptionOutput<TRequest, TResponse> _output;
            private readonly Action<TRequest, TException, MicroProcessorTestContext> _assertMethod;

            public IsExceptionMethod(QueryExceptionOutput<TRequest, TResponse> output, Action<TRequest, TException, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._output.GetInputMessage<TRequest>(), _output._output.IsExceptionOfType<TException>());

            private void Execute(MessageEnvelope<TRequest> request, TException exception) =>
                _assertMethod?.Invoke(request.Content, exception, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly ExceptionOutput _output;

        public QueryExceptionOutput(MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _context = context;
            _output = new ExceptionOutput(exception);
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            new IsExceptionMethod<TException>(this, assertMethod);

        public ITestOutputAssertMethod IsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod) =>
            _output.IsNoException();
    }
}

using System;

namespace Kingo.MicroServices
{
    internal sealed class QueryExceptionOutput<TResponse> : IQueryTestOutput<TResponse>
    {
        #region [====== IsExceptionMethod<TException> ======]

        private sealed class IsExceptionMethod<TException> : ITestOutputAssertMethod where TException : MicroProcessorOperationException
        {
            private readonly QueryExceptionOutput<TResponse> _output;
            private readonly Action<TException, MicroProcessorTestContext> _assertMethod;

            public IsExceptionMethod(QueryExceptionOutput<TResponse> output, Action<TException, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._output.IsExceptionOfType<TException>());

            private void Execute(TException exception) =>
                _assertMethod?.Invoke(exception, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly ExceptionOutput _output;

        public QueryExceptionOutput(MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _context = context;
            _output = new ExceptionOutput(exception);
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            new IsExceptionMethod<TException>(this, assertMethod);

        public ITestOutputAssertMethod IsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod) =>
            _output.IsNoException();
    }
}

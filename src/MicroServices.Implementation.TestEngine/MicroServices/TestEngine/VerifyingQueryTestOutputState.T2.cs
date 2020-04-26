using System;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class VerifyingQueryTestOutputState<TRequest, TResponse> : VerifyingOutputState
    {
        private readonly MicroProcessorTest _test;
        private readonly IQueryTestOutput<TRequest, TResponse> _output;

        public VerifyingQueryTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _test = test;
            _output = new QueryExceptionOutput<TRequest, TResponse>(context, exception);
        }

        public VerifyingQueryTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _test = test;
            _output = new ResponseOutput<TRequest, TResponse>(context, operationId);
        }

        protected override MicroProcessorTest Test =>
            _test;

        public void AssertOutputIsException<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            VerifyThat(_output.IsException(assertMethod));

        public void AssertOutputIsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod) =>
            VerifyThat(_output.IsResponse(assertMethod));
    }
}

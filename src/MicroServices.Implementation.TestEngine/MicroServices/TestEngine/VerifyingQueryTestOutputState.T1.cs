using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class VerifyingQueryTestOutputState<TResponse> : VerifyingOutputState
    {
        private readonly MicroProcessorTest _test;
        private readonly IQueryTestOutput<TResponse> _output;

        public VerifyingQueryTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorOperationException exception)
        {
            _test = test;
            _output = new QueryExceptionOutput<TResponse>(context, exception);
        }

        public VerifyingQueryTestOutputState(MicroProcessorTest test, MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _test = test;
            _output = new ResponseOutput<TResponse>(context, operationId);
        }

        protected override MicroProcessorTest Test =>
            _test;

        public void AssertOutputIsException<TException>(Action<TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            VerifyThat(_output.IsException(assertMethod));

        public void AssertOutputIsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod) =>
            VerifyThat(_output.IsResponse(assertMethod));
    }
}

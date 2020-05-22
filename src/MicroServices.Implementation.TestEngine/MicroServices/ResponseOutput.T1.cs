using System;

namespace Kingo.MicroServices
{
    internal sealed class ResponseOutput<TResponse> : ReturnValueOutput, IQueryTestOutput<TResponse>
    {
        #region [====== IsResponseMethod ======]

        private sealed class IsResponseMethod : ITestOutputAssertMethod
        {
            private readonly ResponseOutput<TResponse> _output;
            private readonly Action<TResponse, MicroProcessorTestContext> _assertMethod;

            public IsResponseMethod(ResponseOutput<TResponse> output, Action<TResponse, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._context.GetResponse<TResponse>(_output._operationId));

            private void Execute(IMessage<TResponse> response) =>
                _assertMethod?.Invoke(response.Content, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly MicroProcessorTestOperationId _operationId;

        public ResponseOutput(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _context = context;
            _operationId = operationId;
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            IsException<TException>();

        public ITestOutputAssertMethod IsResponse(Action<TResponse, MicroProcessorTestContext> assertMethod) =>
            new IsResponseMethod(this, assertMethod);
    }
}

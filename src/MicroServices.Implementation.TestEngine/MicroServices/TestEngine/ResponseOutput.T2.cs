using System;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ResponseOutput<TRequest, TResponse> : ReturnValueOutput, IQueryTestOutput<TRequest, TResponse>
    {
        #region [====== IsResponseMethod ======]

        private sealed class IsResponseMethod : ITestOutputAssertMethod
        {
            private readonly ResponseOutput<TRequest, TResponse> _output;
            private readonly Action<TRequest, TResponse, MicroProcessorTestContext> _assertMethod;

            public IsResponseMethod(ResponseOutput<TRequest, TResponse> output, Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod)
            {
                _output = output;
                _assertMethod = assertMethod;
            }

            public void Execute() =>
                Execute(_output._context.GetInputMessage<TRequest>(_output._operationId), _output._context.GetResponse<TResponse>(_output._operationId));

            private void Execute(IMessage<TRequest> request, IMessage<TResponse> response) =>
                _assertMethod?.Invoke(request.Content, response.Content, _output._context);
        }

        #endregion

        private readonly MicroProcessorTestContext _context;
        private readonly MicroProcessorTestOperationId _operationId;

        public ResponseOutput(MicroProcessorTestContext context, MicroProcessorTestOperationId operationId)
        {
            _context = context;
            _operationId = operationId;
        }

        public ITestOutputAssertMethod IsException<TException>(Action<TRequest, TException, MicroProcessorTestContext> assertMethod) where TException : MicroProcessorOperationException =>
            IsException<TException>();

        public ITestOutputAssertMethod IsResponse(Action<TRequest, TResponse, MicroProcessorTestContext> assertMethod) =>
            new IsResponseMethod(this, assertMethod);
    }
}

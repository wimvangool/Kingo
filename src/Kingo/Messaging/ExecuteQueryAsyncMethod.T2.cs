﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class ExecuteQueryAsyncMethod<TMessageIn, TMessageOut> : ExecuteAsyncMethod<TMessageOut>
    {
        public static Task<TMessageOut> Invoke(MicroProcessor processor, IQuery<TMessageIn, TMessageOut> query, TMessageIn message, CancellationToken? token) =>
            Invoke(new ExecuteQueryAsyncMethod<TMessageIn, TMessageOut>(processor, new QueryContext(processor.Principal, token), query, message));

        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly TMessageIn _message;

        private ExecuteQueryAsyncMethod(MicroProcessor processor, QueryContext context, IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
        {
            Processor = processor;
            Context = context;            

            _query = query;
            _message = message;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override QueryContext Context
        {
            get;
        }

        protected override async Task<ExecuteAsyncResult<TMessageOut>> InvokeQueryCore()
        {
            Context.Messages.Push(MessageInfo.FromQuery(_message));

            try
            {
                return await Processor.Pipeline.Build(new QueryDecorator<TMessageIn, TMessageOut>(Context, _query)).ExecuteAsync(_message, Context);
            }
            finally
            {
                Context.Messages.Pop();
            }
        }            

        protected override BadRequestException NewBadRequestException(InternalProcessorException exception, string message) =>
            exception.AsBadRequestException(_message, message);

        protected override InternalServerErrorException NewInternalServerErrorException(InternalProcessorException exception, string message) =>
            exception.AsInternalServerErrorException(_message, message);

        protected override InternalServerErrorException NewInternalServerErrorException(Exception exception, string message) =>
            new InternalServerErrorException(_message, message, exception);
    }
}

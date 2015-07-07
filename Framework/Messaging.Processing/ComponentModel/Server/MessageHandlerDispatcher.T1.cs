using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syztem.ComponentModel.Server.Domain;
using Syztem.Security;

namespace Syztem.ComponentModel.Server
{
    internal sealed class MessageHandlerDispatcher<TMessage> : MessageHandlerDispatcher where TMessage : class, IMessage<TMessage>
    {       
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;        

        internal MessageHandlerDispatcher(TMessage message, IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _message = message;
            _handler = handler;
            _processor = processor;            
        }

        public override IMessage Message
        {
            get { return _message; }
        }

        public async override Task InvokeAsync()
        {
            ThrowIfCancellationRequested();

            using (var scope = _processor.CreateUnitOfWorkScope())
            {
                await InvokeAsync(_message);
                await scope.CompleteAsync();
            }
            ThrowIfCancellationRequested();
        }                       

        private async Task InvokeAsync(TMessage message)
        {
            if (_handler == null)
            {
                var messageHandlerFactory = _processor.MessageHandlerFactory;
                if (messageHandlerFactory == null)
                {
                    return;
                }
                var source = MessageProcessor.CurrentMessage.DetermineMessageSourceOf(message);

                foreach (var handler in messageHandlerFactory.CreateMessageHandlersFor(message, source))
                {
                    await InvokeAsync(message, handler);
                }
            }
            else
            {
                await InvokeAsync(message, new MessageHandlerInstance<TMessage>(_handler));
            }            
        }

        private async Task InvokeAsync(TMessage message, MessageHandlerInstance<TMessage> handler)
        {
            var messageHandler = new MessageHandlerWrapper<TMessage>(message, handler);            
            var pipeline = _processor.BuildCommandOrEventHandlerPipeline().ConnectTo(messageHandler);

            try
            {
                await pipeline.InvokeAsync();
            }
            catch (AggregateException exception)
            {
                FunctionalException functionalException;

                if (TryConvertToFunctionalException(messageHandler, exception, out functionalException))
                {
                    throw functionalException;
                }
                throw;
            }
            catch (DomainException exception)
            {
                FunctionalException functionalException;

                if (TryConvertToFunctionalException(messageHandler, exception, out functionalException))
                {
                    throw functionalException;
                }
                throw;
            }
            ThrowIfCancellationRequested();
        }

        private static bool TryConvertToFunctionalException(IMessageHandler handler, AggregateException exception, out FunctionalException functionalException)
        {
            var functionalExceptions = new List<FunctionalException>();

            // NB: Handle will throw a new AggregateException if not all inner-exceptions are handled.
            exception.Flatten().Handle(innerException =>
            {
                var domainException = innerException as DomainException;
                if (domainException != null)
                {
                    FunctionalException convertedException;

                    if (TryConvertToFunctionalException(handler, domainException, out convertedException))
                    {
                        functionalExceptions.Add(convertedException);
                        return true;
                    }
                }                
                return false;
            });

            // When one or more exceptions were converted into a FunctionalException, the most prominent
            // one is returned.
            if (functionalExceptions.Count == 0)
            {
                functionalException = null;
                return false;
            }
            if (functionalExceptions.Count == 1)
            {
                functionalException = functionalExceptions[0];
                return true;
            }
            return
                TryGetFirst<SenderNotAuthorizedException>(functionalExceptions, out functionalException) ||
                TryGetFirst<InvalidMessageException>(functionalExceptions, out functionalException) ||
                TryGetFirst<CommandExecutionException>(functionalExceptions, out functionalException);
        }

        private static bool TryGetFirst<TException>(IEnumerable<FunctionalException> functionalExceptions, out FunctionalException functionalException) where TException : FunctionalException
        {
            var exceptions = from exception in functionalExceptions
                             let desiredException = exception as TException
                             where desiredException != null
                             select desiredException;

            return (functionalException = exceptions.FirstOrDefault()) != null;
        }        
        
        private static bool TryConvertToFunctionalException(IMessageHandler handler, DomainException exception, out FunctionalException functionalException)
        {
            foreach (var exceptionFilter in GetDomainExceptionFilters(handler))
            {
                if (exceptionFilter.TryConvertToFunctionalException(handler.Message, exception, out functionalException))
                {
                    return true;
                }
            }
            functionalException = null;
            return false;
        }

        private static IEnumerable<IDomainExceptionFilter> GetDomainExceptionFilters(IMessageHandler handler)
        {
            var classAttributes = handler.GetClassAttributesOfType<IDomainExceptionFilter>();
            var methodAttributes = handler.GetMethodAttributesOfType<IDomainExceptionFilter>();

            return classAttributes.Concat(methodAttributes);
        }
    }
}

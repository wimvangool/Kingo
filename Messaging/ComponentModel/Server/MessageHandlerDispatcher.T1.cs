using System.Collections.Generic;
using System.ComponentModel.Server.Domain;
using System.Linq;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerDispatcher<TMessage> : IMessageHandler where TMessage : class, IMessage<TMessage>
    {       
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;
        private readonly List<CatchAttribute> _catchAttributes;

        internal MessageHandlerDispatcher(TMessage message, IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _message = message;
            _handler = handler;
            _processor = processor;
            _catchAttributes = new List<CatchAttribute>();
        }

        public IMessage Message
        {
            get { return _message; }
        }

        public void Invoke()
        {
            try
            {
                HandleMessage();    
            }
            catch (AggregateException exception)
            {
                if (MatchesAll(exception, _catchAttributes))
                {
                    throw new CommandExecutionException(_message, null, exception);
                }
                throw;
            }
            catch (ConstraintViolationException exception)
            {
                if (Matches(exception, _catchAttributes))
                {
                    throw exception.AsCommandExecutionException(_message);
                }
                throw;
            }
            finally
            {
                _catchAttributes.Clear();
            }
        }        

        private void HandleMessage()
        {
            _processor.Message.ThrowIfCancellationRequested();

            using (var scope = _processor.CreateUnitOfWorkScope())
            {
                HandleMessage(_message);

                scope.Complete();
            }
            _processor.Message.ThrowIfCancellationRequested();
        }

        private void HandleMessage(TMessage message)
        {
            if (_handler == null)
            {
                if (_processor.MessageHandlerFactory == null)
                {
                    return;
                }
                var source = _processor.Message.DetermineMessageSourceOf(message);

                foreach (var handler in _processor.MessageHandlerFactory.CreateMessageHandlersFor(message, source))
                {
                    HandleMessage(message, handler);
                }
            }
            else
            {
                HandleMessage(message, new MessageHandlerInstance<TMessage>(_handler));
            }
        }

        private void HandleMessage(TMessage message, MessageHandlerInstance<TMessage> handler)
        {
            var messageHandler = new MessageHandlerWrapper<TMessage>(message, handler);

            foreach (var catchAttribute in handler.GetMethodAttributesOfType<CatchAttribute>())
            {
                _catchAttributes.Add(catchAttribute);
            }
            _processor.BuildCommandOrEventHandlerPipeline().ConnectTo(messageHandler).Invoke();                               
            _processor.Message.ThrowIfCancellationRequested();
        }        

        private static bool MatchesAll(AggregateException exception, IEnumerable<CatchAttribute> attributes)
        {            
            return
                exception != null &&
                exception.InnerExceptions.Count > 0 &&
                exception.InnerExceptions.All(innerException => Matches(innerException, attributes));
        }

        private static bool Matches(Exception exception, IEnumerable<CatchAttribute> attributes)
        {
            return
                Matches(exception as ConstraintViolationException, attributes) ||
                MatchesAll(exception as AggregateException, attributes);
        }

        private static bool Matches(ConstraintViolationException exception, IEnumerable<CatchAttribute> attributes)
        {
            return exception != null && attributes.Any(attribute => attribute.ExceptionType.IsInstanceOfType(exception));
        }
    }
}

using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a handler of arbitrary commands.
    /// </summary>    
    public sealed class MessageProcessor : IMessageProcessor
    {        
        private readonly MessageHandlerFactory _handlerFactory;
        private readonly MessageHandlerPipelineFactory _pipelineFactory;               

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="handlerFactory">The factory used to instantiate the message-handlers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlerFactory"/> is <c>null</c>.
        /// </exception>
        public MessageProcessor(MessageHandlerFactory handlerFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException("handlerFactory");
            }
            _handlerFactory = handlerFactory;            
            _pipelineFactory = new MessageHandlerPipelineFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor" /> class.
        /// </summary>
        /// <param name="handlerFactory">The factory used to instantiate the message-handlers.</param>
        /// <param name="pipelineFactory">The factory used to build a (custom) pipeline.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlerFactory"/> is <c>null</c>.        
        /// </exception>
        public MessageProcessor(MessageHandlerFactory handlerFactory, MessageHandlerPipelineFactory pipelineFactory)
        {
            if (handlerFactory == null)
            {
                throw new ArgumentNullException("handlerFactory");
            }
            _handlerFactory = handlerFactory;           
            _pipelineFactory = pipelineFactory ?? new MessageHandlerPipelineFactory();
        }        

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message) where TMessage : class
        {
            Handle(message, false);
        }

        internal void Handle<TMessage>(TMessage message, bool isInternalMessage) where TMessage : class
        {            
            using (var scope = CreateMessageProcessorContextScope())
            {
                var context = MessageProcessorContext.Current;

                foreach (var handler in CreateMessageHandlersFor(message, isInternalMessage))
                {                    
                    BuildCommand(handler, message, context).Execute();
                }
                scope.Complete();
            }
        }

        private IEnumerable<IMessageHandler<TMessage>> CreateMessageHandlersFor<TMessage>(TMessage message, bool isInternalMessage) where TMessage : class
        {
            return isInternalMessage
                ? _handlerFactory.CreateInternalHandlersFor(message)
                : _handlerFactory.CreateExternalHandlersFor(message);
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, IExternalMessageHandler<TMessage> handler) where TMessage : class
        {
            using (var scope = CreateMessageProcessorContextScope())
            {                
                BuildCommand(new ExternalMessageHandler<TMessage>(handler), message, MessageProcessorContext.Current).Execute();
                scope.Complete();
            }
        }

        /// <inheritdoc />
        public void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            Handle(message, new ActionHandler<TMessage>(action));
        }

        private MessageProcessorContextScope CreateMessageProcessorContextScope()
        {
            return new MessageProcessorContextScope(this);
        }                    

        private IMessageCommand BuildCommand<TMessage>(IMessageHandler<TMessage> handler, TMessage message, MessageProcessorContext context)
            where TMessage : class
        {
            return _pipelineFactory.CreatePipeline(handler, message, context);
        }                              
  
        /// <summary>
        /// Indicates whether or not a message of the specified type is currently being handled.
        /// </summary>
        /// <typeparam name="TMessage">Type of a message.</typeparam>
        /// <returns>
        /// <c>true</c> if a message of the specified type is currently being handled; otherwise <c>false</c>.
        /// </returns>
        public static bool IsCurrentlyHandling<TMessage>()
        {
            return IsCurrentlyHandling(typeof(TMessage));
        }

        /// <summary>
        /// Indicates whether or not a message of the specified type is currently being handled.
        /// </summary>
        /// <param name="messageType">Type of a message.</param>
        /// <returns>
        /// <c>true</c> if a message of the specified type is currently being handled; otherwise <c>false</c>.
        /// </returns>
        public static bool IsCurrentlyHandling(Type messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            var context = MessageProcessorContext.Current;
            if (context == null)
            {
                return false;
            }
            return context.HasMessageOnStackOfType(messageType);
        }
    }
}

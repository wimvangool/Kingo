using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MessageHandlerInstance : IMessageHandlerFactory
    {
        public IEnumerable<MessageHandler> ResolveMessageHandlers<TMessage>(TMessage message, MessageHandlerContext context)
        {
            if (TryCreateMessageHandler(message, context, out var messageHandler))
            {
                yield return messageHandler;
            }
        }

        public abstract bool TryCreateMessageHandler<TMessage>(TMessage message, MessageHandlerContext context, out MessageHandler handler);

        public static IEnumerable<MessageHandlerInstance> FromHandler(object handler, MicroProcessorOperationTypes? operationTypes = null)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (MessageHandlerClass.IsMessageHandlerClass(handler.GetType(), out var messageHandlerClass))
            {
                foreach (var messageHandlerInterface in messageHandlerClass.Interfaces)
                {
                    yield return FromHandler(handler, operationTypes, messageHandlerInterface);
                }
            }
        }

        private static readonly ConcurrentDictionary<MessageHandlerInterface, Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance>> _InstanceFactories =
            new ConcurrentDictionary<MessageHandlerInterface, Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance>>();

        private static MessageHandlerInstance FromHandler(object handler, MicroProcessorOperationTypes? operationTypes, MessageHandlerInterface messageHandlerInterface) =>
            _InstanceFactories.GetOrAdd(messageHandlerInterface, CreateInstanceFactory).Invoke(handler, operationTypes);

        private static Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance> CreateInstanceFactory(MessageHandlerInterface messageHandlerInterface)
        {            
            var handlerParameter = Expression.Parameter(typeof(object), "handler");
            var handlerOfExpectedType = Expression.Convert(handlerParameter, messageHandlerInterface.InterfaceType);
            var operationTypesParameter = Expression.Parameter(typeof(MicroProcessorOperationTypes?), "operationTypes");

            var messageHandlerInstanceTypeDefinition = typeof(MessageHandlerInstance<>);
            var messageHandlerInstanceType = messageHandlerInstanceTypeDefinition.MakeGenericType(messageHandlerInterface.MessageType);
            var messageHandlerInstanceConstructorParameters = new[] { messageHandlerInterface.InterfaceType, typeof(MicroProcessorOperationTypes?) };
            var messageHandlerInstanceConstructor = messageHandlerInstanceType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, messageHandlerInstanceConstructorParameters, null);
            var newMessageHandlerInstanceExpression = Expression.New(messageHandlerInstanceConstructor, handlerOfExpectedType, operationTypesParameter);
            var createMessageHandlerInstanceExpression = Expression.Lambda<Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance>>(
                Expression.Convert(newMessageHandlerInstanceExpression, typeof(MessageHandlerInstance)),
                handlerParameter,
                operationTypesParameter);

            return createMessageHandlerInstanceExpression.Compile();
        }        
    }
}

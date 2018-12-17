using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Kingo.MicroServices
{
    internal abstract class MessageHandlerInstance
    {
        public abstract bool TryCreateMessageHandler<TMessage>(TMessage message, MessageHandlerContext context, out MessageHandler handler);

        public static IEnumerable<MessageHandlerInstance> FromHandler(object handler, MicroProcessorOperationTypes? operationTypes = null)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            if (MessageHandlerClass.IsMessageHandlerClass(handler.GetType(), out var messageHandlerClass))
            {
                foreach (var interfaceType in messageHandlerClass.Interfaces)
                {
                    yield return FromHandler(handler, operationTypes, interfaceType);
                }
            }
        }

        private static readonly ConcurrentDictionary<Type, Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance>> _InstanceFactories =
            new ConcurrentDictionary<Type, Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance>>();

        private static MessageHandlerInstance FromHandler(object handler, MicroProcessorOperationTypes? operationTypes, Type interfaceType) =>
            _InstanceFactories.GetOrAdd(interfaceType, CreateInstanceFactory).Invoke(handler, operationTypes);

        private static Func<object, MicroProcessorOperationTypes?, MessageHandlerInstance> CreateInstanceFactory(Type interfaceType)
        {
            var messageType = MessageHandlerClass.GetMessageTypeOf(interfaceType);            

            var handlerParameter = Expression.Parameter(typeof(object), "handler");
            var handlerOfExpectedType = Expression.Convert(handlerParameter, interfaceType);
            var operationTypesParameter = Expression.Parameter(typeof(MicroProcessorOperationTypes?), "operationTypes");

            var messageHandlerInstanceTypeDefinition = typeof(MessageHandlerInstance<>);
            var messageHandlerInstanceType = messageHandlerInstanceTypeDefinition.MakeGenericType(messageType);
            var messageHandlerInstanceConstructorParameters = new[] { interfaceType, typeof(MicroProcessorOperationTypes?) };
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MessageHandlerInstance : MessageHandler, IHandleAsyncMethodFactory
    {
        private readonly IMessageHandlerConfiguration _configuration;

        protected MessageHandlerInstance(MicroProcessorComponent component, MessageHandlerInterface @interface, IMessageHandlerConfiguration configuration) :
            base(component, @interface)
        {
            _configuration = configuration;
        }

        public override bool HandlesExternalMessages =>
            _configuration?.HandlesExternalMessages ?? base.HandlesExternalMessages;

        public override bool HandlesInternalMessages =>
            _configuration?.HandlesInternalMessages ?? base.HandlesInternalMessages;

        public IEnumerable<HandleAsyncMethod<TMessage>> CreateMethodsFor<TMessage>(MicroProcessorOperationKinds operationKind, IServiceProvider serviceProvider)
        {
            if (TryCreateMethod(operationKind, out HandleAsyncMethod<TMessage> method))
            {
                yield return method;
            }
        }        

        public abstract bool TryCreateMethod<TMessage>(MicroProcessorOperationKinds operationKind, out HandleAsyncMethod<TMessage> method);

        #region [====== FromInstance ======]

        private static readonly ConcurrentDictionary<MessageHandlerInterface, Func<object, MessageHandlerInstance>> _InstanceFactories =
            new ConcurrentDictionary<MessageHandlerInterface, Func<object, MessageHandlerInstance>>();

        public static IEnumerable<MessageHandlerInstance> FromInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (MessageHandlerType.IsMessageHandlerComponent(instance.GetType(), out var messageHandlerClass))
            {
                foreach (var messageHandlerInterface in messageHandlerClass.Interfaces)
                {
                    yield return FromInstance(instance, messageHandlerInterface);
                }
            }
        }        

        private static MessageHandlerInstance FromInstance(object handler, MessageHandlerInterface messageHandlerInterface) =>
            _InstanceFactories.GetOrAdd(messageHandlerInterface, CreateInstanceFactory).Invoke(handler);

        private static Func<object, MessageHandlerInstance> CreateInstanceFactory(MessageHandlerInterface messageHandlerInterface)
        {            
            var messageHandlerParameter = Expression.Parameter(typeof(object), "messageHandler");
            var messageHandlerOfExpectedType = Expression.Convert(messageHandlerParameter, messageHandlerInterface.Type);            

            var messageHandlerInstanceTypeDefinition = typeof(MessageHandlerInstance<>);
            var messageHandlerInstanceType = messageHandlerInstanceTypeDefinition.MakeGenericType(messageHandlerInterface.MessageType);
            var messageHandlerInstanceConstructorParameters = new[] { messageHandlerInterface.Type };
            var messageHandlerInstanceConstructor = messageHandlerInstanceType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, messageHandlerInstanceConstructorParameters, null);
            var newMessageHandlerInstanceExpression = Expression.New(messageHandlerInstanceConstructor, messageHandlerOfExpectedType);
            var createMessageHandlerInstanceExpression = Expression.Lambda<Func<object, MessageHandlerInstance>>(
                Expression.Convert(newMessageHandlerInstanceExpression, typeof(MessageHandlerInstance)),
                messageHandlerParameter);

            return createMessageHandlerInstanceExpression.Compile();
        }

        #endregion
    }
}

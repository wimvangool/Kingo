using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal static class EventProcessor
    {
        #region [====== HandleEventsAsync ======]

        public static async Task<MessageHandlerOperationResult<TMessage>> HandleEventsAsync<TMessage>(MessageHandlerOperationResult<TMessage> result, MessageHandlerOperationContext context)
        {
            await foreach (var eventResult in HandleEventsAsync(result.AsEnumerable(), context).ConfigureAwait(false))
            {
                result = result.Append(eventResult);
            }
            return result;
        }

        private static async IAsyncEnumerable<MessageHandlerOperationResult> HandleEventsAsync(IEnumerable<Message<object>> messages, MessageHandlerOperationContext context)
        {
            foreach (var @event in messages.Where(IsUnscheduledEvent))
            {
                yield return await HandleEventAsync(@event, context).ConfigureAwait(false);
            }
        }

        private static Task<MessageHandlerOperationResult> HandleEventAsync(Message<object> @event, MessageHandlerOperationContext context) =>
            _HandleEventAsyncMethods.GetOrAdd(@event.Content.GetType(), CreateHandleEventAsyncMethod).InvokeAsync(@event, context);

        private static bool IsUnscheduledEvent(IMessage message) =>
            message.Kind == MessageKind.Event && message.DeliveryTimeUtc == null;

        #endregion

        #region [====== HandleEventAsyncMethod ======]

        private abstract class HandleEventAsyncMethod
        {
            public abstract Task<MessageHandlerOperationResult> InvokeAsync(Message<object> @event, MessageHandlerOperationContext context);
        }

        private sealed class HandleEventAsyncMethod<TEvent> : HandleEventAsyncMethod
        {
            public override async Task<MessageHandlerOperationResult> InvokeAsync(Message<object> @event, MessageHandlerOperationContext context) =>
                await new MessageHandlerOperation<TEvent>(context, @event.ConvertTo<TEvent>()).ExecuteAsync().ConfigureAwait(false);
        }

        private static readonly ConcurrentDictionary<Type, HandleEventAsyncMethod> _HandleEventAsyncMethods = new ConcurrentDictionary<Type, HandleEventAsyncMethod>();

        private static HandleEventAsyncMethod CreateHandleEventAsyncMethod(Type messageType)
        {
            var methodDefinition = typeof(HandleEventAsyncMethod<>);
            var method = methodDefinition.MakeGenericType(messageType);
            return (HandleEventAsyncMethod) Activator.CreateInstance(method);
        }

        #endregion
    }
}

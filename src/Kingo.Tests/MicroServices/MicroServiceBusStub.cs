using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusStub : MicroServiceBus, IReadOnlyList<object>
    {
        private readonly List<object> _events;

        public MicroServiceBusStub()
        {
            _events = new List<object>();
        }

        public int Count =>
            _events.Count;

        public object this[int index] =>
            _events[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<object> GetEnumerator() =>
            _events.GetEnumerator();        

        public override Task PublishAsync(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _events.Add(message);
            return Task.CompletedTask;
        }

        public void AssertEventCountIs(int count) =>
            Assert.AreEqual(count, Count, $"Unexpected number of events published: expected: {count}, but was {Count}.");

        public void AssertEvent<TEvent>(int index, Action<TEvent> callback = null)
        {
            try
            {
                AssertEvent((TEvent) this[index], callback);
            }
            catch (IndexOutOfRangeException)
            {
                Assert.Fail($"No event found at index {index}.");
            }
            catch (InvalidCastException)
            {
                Assert.Fail($"Event at index {index} is not of type {typeof(TEvent).FriendlyName()}.");
            }
        }

        private static void AssertEvent<TEvent>(TEvent @event, Action<TEvent> callback) =>
            callback?.Invoke(@event);      

        private static readonly Context<MicroServiceBusStub> _Context = new Context<MicroServiceBusStub>(null);

        public static MicroServiceBusStub Current =>
            _Context.Current;

        public static IDisposable CreateContext() =>
            _Context.OverrideAsyncLocal(new MicroServiceBusStub());        
    }
}

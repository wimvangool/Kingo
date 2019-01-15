using System;
using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusStub : MemoryServiceBus
    {                
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

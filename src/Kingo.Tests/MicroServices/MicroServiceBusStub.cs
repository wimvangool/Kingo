using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    }
}

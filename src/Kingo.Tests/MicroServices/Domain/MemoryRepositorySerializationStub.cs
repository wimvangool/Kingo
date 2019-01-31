using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    public sealed class MemoryRepositorySerializationStub : MemoryRepository<Guid, int, NumberSnapshot, Number>
    {
        private readonly Dictionary<Guid, AggregateReadSet> _dataSets;
        private readonly List<IChangeSet<Guid, int, NumberSnapshot>> _changeSets;

        public MemoryRepositorySerializationStub(SerializationStrategy serializationStrategy)
            : base(serializationStrategy)
        {
            _dataSets = new Dictionary<Guid, AggregateReadSet>();
            _changeSets = new List<IChangeSet<Guid, int, NumberSnapshot>>();
        }

        public void Add(Guid id, AggregateReadSet dataSet) =>
            _dataSets.Add(id, dataSet);

        protected internal override async Task<AggregateReadSet> SelectByIdAsync(Guid id)
        {
            if (_dataSets.TryGetValue(id, out var dataSet))
            {
                return dataSet;
            }
            return await base.SelectByIdAsync(id);
        }

        public void AssertChangeSet(int index, Action<IChangeSet<Guid, int, NumberSnapshot>> callback) =>
            callback.Invoke(_changeSets[index]);

        protected internal override Task FlushAsync(IChangeSet<Guid, int, NumberSnapshot> changeSet)
        {
            _changeSets.Add(changeSet);
            return base.FlushAsync(changeSet);
        }
    }
}

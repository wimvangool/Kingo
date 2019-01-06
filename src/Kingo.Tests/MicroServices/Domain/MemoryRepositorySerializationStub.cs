using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Domain
{
    public sealed class MemoryRepositorySerializationStub : MemoryRepository<Guid, int, Number>
    {
        private readonly Dictionary<Guid, AggregateDataSet> _dataSets;
        private readonly List<IChangeSet<Guid, int>> _changeSets;

        public MemoryRepositorySerializationStub(SerializationStrategy serializationStrategy)
            : base(serializationStrategy)
        {
            _dataSets = new Dictionary<Guid, AggregateDataSet>();
            _changeSets = new List<IChangeSet<Guid, int>>();
        }

        public void Add(Guid id, AggregateDataSet dataSet) =>
            _dataSets.Add(id, dataSet);

        protected internal override async Task<AggregateDataSet> SelectByIdAsync(Guid id)
        {
            if (_dataSets.TryGetValue(id, out var dataSet))
            {
                return dataSet;
            }
            return await base.SelectByIdAsync(id);
        }

        public void AssertChangeSet(int index, Action<IChangeSet<Guid, int>> callback) =>
            callback.Invoke(_changeSets[index]);

        protected internal override Task FlushAsync(IChangeSet<Guid, int> changeSet)
        {
            _changeSets.Add(changeSet);
            return base.FlushAsync(changeSet);
        }
    }
}

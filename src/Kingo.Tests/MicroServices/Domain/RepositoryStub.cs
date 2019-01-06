using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices.Domain
{
    internal sealed class RepositoryStub : Repository<Guid, int, Number>
    {
        private readonly Dictionary<Guid, AggregateDataSet<Guid>> _dataSets;

        public RepositoryStub(SerializationStrategy serializationStrategy)
            : base(serializationStrategy)
        {
            _dataSets = new Dictionary<Guid, AggregateDataSet<Guid>>();
        }

        public IChangeSet<Guid, int> LastChangeSet
        {
            get;
            private set;
        }

        public void Add(AggregateDataSet<Guid> dataSet) =>
            _dataSets.Add(dataSet.Id, dataSet);

        protected internal override Task<AggregateDataSet<Guid>> SelectByIdAsync(Guid id) => AsyncMethod.Run(() =>
        {
            if (_dataSets.TryGetValue(id, out var dataSet))
            {
                return dataSet;
            }
            return null;
        });

        protected internal override Task FlushAsync(IChangeSet<Guid, int> changeSet)
        {
            LastChangeSet = changeSet;
            return Task.CompletedTask;
        }
    }
}

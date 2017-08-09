using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Domain
{
    public sealed class MemoryRepositorySpy<TKey, TAggregate> : MemoryRepository<TKey, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        private readonly List<IChangeSet<TKey>> _changeSets;        
        private int _requiresFlushCount;
        private int _flushCount;

        public MemoryRepositorySpy(IEnumerable<TAggregate> aggregates) :
            base(MemoryRepositoryBehavior.StoreSnapshots, aggregates)
        {
            _changeSets = new List<IChangeSet<TKey>>();           
        }

        public override bool RequiresFlush()
        {
            _requiresFlushCount++;

            return base.RequiresFlush();
        }

        public override Task FlushAsync()
        {
            _flushCount++;

            return base.FlushAsync();
        }

        protected internal override Task FlushAsync(IChangeSet<TKey> changeSet)
        {
            _changeSets.Add(changeSet);

            return base.FlushAsync(changeSet);
        }

        public void AssertRequiresFlushCountIs(int count)
        {
            Assert.AreEqual(count, _requiresFlushCount);
        }

        public void AssertFlushCountIs(int count)
        {
            Assert.AreEqual(count, _flushCount);
        }

        public IChangeSet<TKey> GetChangeSet(int index) =>
            _changeSets[index];
    }
}

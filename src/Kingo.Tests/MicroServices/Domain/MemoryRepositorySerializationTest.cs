using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public abstract class MemoryRepositorySerializationTest
    {
        #region [====== Aggregates ======]

        protected sealed class AggregateOfWrongType : AggregateRoot<Guid, int>
        {
            public AggregateOfWrongType(ISnapshotOrEvent<Guid, int> snapshotOrEvent) :
                base(null, snapshotOrEvent, false) { }

            protected override int NextVersion() =>
                Version + 1;
        }

        #endregion

        [TestInitialize]
        public virtual void Setup()
        {
            Repository = CreateRepository();
        }

        protected MemoryRepositorySerializationStub Repository
        {
            get;
            private set;
        }

        protected abstract MemoryRepositorySerializationStub CreateRepository();

        #region [====== Read Methods ======]

        [TestMethod]
        [ExpectedException(typeof(CouldNotRestoreAggregateException))]
        public async Task GetByIdAsync_Throws_IfSelectedDataSetIsEmpty()
        {
            var numberId = Guid.NewGuid();

            Repository.Add(numberId, new AggregateDataSet());

            await Repository.GetByIdAsync(numberId);
        }

        protected abstract Number RandomNumber(int value = 0, IEventBus eventBus = null);

        #endregion       
    }
}

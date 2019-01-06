using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Domain
{
    [TestClass]
    public sealed class MemoryRepositoryUsingEventsAndSnapshotsTest : MemoryRepositorySerializationTest
    {
        #region [====== Write Methods ======]



        #endregion

        protected override MemoryRepositorySerializationStub CreateRepository() =>
            new MemoryRepositorySerializationStub(SerializationStrategy.UseEvents(3));

        protected override Number RandomNumber(int value = 0, IEventBus eventBus = null) =>
            NumberUsingEvents.CreateNumber(Guid.NewGuid(), value, eventBus);
    }
}

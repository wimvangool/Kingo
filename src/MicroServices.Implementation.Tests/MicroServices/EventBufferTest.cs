using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class EventBufferTest
    {        
        #region [====== Count ======]

        [TestMethod]
        public void Count_ReturnsZero_IsStreamIsEmpty()
        {
            Assert.AreEqual(0, EventBuffer.Empty.Count);
        }

        [TestMethod]
        public void Count_ReturnsExpectedValue_IfStreamIsNotEmpty()
        {
            var numberGenerator = new Random();
            var itemsToAdd = numberGenerator.Next(1, 100);
            var itemToAdd = numberGenerator.Next();
            var buffer = new EventBuffer(Enumerable.Repeat(CreateMessage(itemToAdd), itemsToAdd));

            Assert.AreEqual(itemsToAdd, buffer.Count);
        }

        #endregion

        #region [====== Indexer ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsNegative()
        {
            EventBuffer.Empty[-1].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsZero_And_StreamIsEmpty()
        {
            EventBuffer.Empty[0].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsGreaterThanOrEqualToCount()
        {
            EventBuffer.Empty[1].IgnoreValue();
        }

        #endregion

        #region [====== Enumerable ======]

        [TestMethod]
        public void GetEnumerator_ReturnsNoItems_IfStreamIsEmpty()
        {
            Assert.AreEqual(0, EventBuffer.Empty.AsEnumerable().Count());
        }

        [TestMethod]
        public void GetEnumerator_ReturnsExpectedItems_IfStreamIsNotEmpty()
        {
            var numberGenerator = new Random();
            var itemsToAdd = numberGenerator.Next(1, 100);
            var itemToAdd = numberGenerator.Next();
            var buffer = new EventBuffer(Enumerable.Repeat(CreateMessage(itemToAdd), itemsToAdd));

            Assert.AreEqual(itemsToAdd, buffer.AsEnumerable().Count());
            Assert.IsTrue(buffer.All(item => item.Instance.Equals(itemToAdd)));
        }

        #endregion        

        #region [====== HandleWithAsync ======]        

        [TestMethod]        
        public async Task HandleWithAsync_HandlesNoMessages_IfStreamIsEmpty()
        {
            var processor = new MessageProcessorSpy();

            await EventBuffer.Empty.HandleWith(processor, null);

            Assert.AreEqual(0, processor.Count);
        }

        [TestMethod]
        public async Task HandleWithAsync_HandlesExpectedMessages_IfStreamIsNotEmpty()
        {
            var numberGenerator = new Random();
            var itemsToAdd = numberGenerator.Next(1, 100);
            var itemToAdd = numberGenerator.Next();
            var buffer = new EventBuffer(Enumerable.Repeat(CreateMessage(itemToAdd), itemsToAdd));
            var processor = new MessageProcessorSpy();

            await buffer.HandleWith(processor, null);

            Assert.AreEqual(itemsToAdd, processor.Count);
            Assert.IsTrue(processor.All(item => item.Equals(itemToAdd)));
        }

        #endregion             

        private static IMessage CreateMessage<TMessage>(TMessage message) =>
            new Event<TMessage>(message);
    }
}

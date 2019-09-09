using System;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageBufferTest
    {        
        #region [====== Count ======]

        [TestMethod]
        public void Count_ReturnsZero_IsStreamIsEmpty()
        {
            Assert.AreEqual(0, MessageBuffer.Empty.Count);
        }

        [TestMethod]
        public void Count_ReturnsExpectedValue_IfStreamIsNotEmpty()
        {
            var buffer = CreateEventBuffer();

            Assert.IsTrue(buffer.Count > 0);
        }

        #endregion

        #region [====== Indexer ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsNegative()
        {
            MessageBuffer.Empty[-1].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsZero_And_StreamIsEmpty()
        {
            MessageBuffer.Empty[0].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsGreaterThanOrEqualToCount()
        {
            MessageBuffer.Empty[1].IgnoreValue();
        }

        #endregion

        #region [====== Enumerable ======]

        [TestMethod]
        public void GetEnumerator_ReturnsNoItems_IfStreamIsEmpty()
        {
            Assert.AreEqual(0, MessageBuffer.Empty.AsEnumerable().Count());
        }

        [TestMethod]
        public void GetEnumerator_ReturnsExpectedItems_IfStreamIsNotEmpty()
        {
            var buffer = CreateCommandBuffer();

            Assert.IsTrue(buffer.All(item => item.Content is int));
        }

        #endregion        

        #region [====== HandleWithAsync ======]        

        [TestMethod]        
        public async Task HandleWithAsync_HandlesNoMessages_IfStreamIsEmpty()
        {
            var processor = new MessageProcessorSpy();

            await MessageBuffer.Empty.HandleEventsWith(processor, null);

            Assert.AreEqual(0, processor.Count);
        }

        [TestMethod]
        public async Task HandleWithAsync_HandlesExpectedMessages_IfStreamIsNotEmpty()
        {
            var deliveryTime = Clock.Current.LocalDateAndTime().AddDays(2);

            var commandsWithoutDeliveryTime = CreateCommandBuffer();
            var commandsWithDeliveryTime = CreateCommandBuffer(deliveryTime);
            var eventsWithoutDeliveryTime = CreateEventBuffer();
            var eventsWithDeliveryTime = CreateEventBuffer(deliveryTime);

            var messageBuffer = commandsWithoutDeliveryTime
                .Append(commandsWithDeliveryTime)
                .Append(eventsWithoutDeliveryTime)
                .Append(eventsWithDeliveryTime);

            var processor = new MessageProcessorSpy();

            await messageBuffer.HandleEventsWith(processor, null);

            Assert.AreEqual(eventsWithoutDeliveryTime.Count, processor.Count);
            Assert.IsTrue(processor.All(item => item.Equals(eventsWithoutDeliveryTime.First().Content)));
        }

        #endregion

        private static readonly Random _NumberGenerator = new Random();

        private static MessageBuffer CreateCommandBuffer(DateTimeOffset? deliveryTime = null) =>
            CreateMessageBuffer(value => MessageToDispatch.CreateCommand(value, deliveryTime));

        private static MessageBuffer CreateEventBuffer(DateTimeOffset? deliveryTime = null) =>
            CreateMessageBuffer(value => MessageToDispatch.CreateEvent(value, deliveryTime));

        private static MessageBuffer CreateMessageBuffer(Func<int, MessageToDispatch> messageFactory)
        {
            lock (_NumberGenerator)
            {
                var messagesToAdd = _NumberGenerator.Next(1, 100);
                var message = _NumberGenerator.Next();
                var messages = Enumerable.Repeat(messageFactory.Invoke(message), messagesToAdd);
                return new MessageBuffer(messages);
            }
        }
    }
}

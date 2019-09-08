using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageStreamTest
    {
        #region [====== MessageStreamStub ======]

        private sealed class MessageStreamStub : MessageStream
        {
            public MessageStreamStub(MessageStream stream) :
                base(stream) { }

            public new TMessage GetMessage<TMessage>(int index) =>
                base.GetMessage<TMessage>(index);
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfEventsIsNull()
        {
            new MessageStream(null);
        }

        #endregion

        #region [====== AssertEvent ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertEvent_Throws_IfIndexIsNegative()
        {
            MessageStream.Empty.AssertEvent<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AssertEvent_Throws_IfIndexIsOutOfRange()
        {
            MessageStream.Empty.AssertEvent<object>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AssertEvent_Throws_IfEventIsNotOfExpectedType()
        {
            CreateEventStream(string.Empty).AssertEvent<int>(0);
        }

        [TestMethod]        
        public void AssertEvent_InvokesSpecifiedAssertion_IfEventIsOfDerivedType()
        {
            var counter = new InvocationCounter();

            CreateEventStream(string.Empty).AssertEvent<object>(0, @event =>
            {
                Assert.AreSame(string.Empty, @event);
                counter.Increment();
            });

            counter.AssertExactly(1);
        }

        [TestMethod]
        public void AssertEvent_InvokesSpecifiedAssertion_IfEventIsOfExpectedType()
        {
            var counter = new InvocationCounter();

            CreateEventStream(string.Empty).AssertEvent<string>(0, @event =>
            {
                Assert.AreSame(string.Empty, @event);
                counter.Increment();
            });

            counter.AssertExactly(1);
        }

        #endregion

        #region [====== GetMessage ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetMessage_Throws_IfIndexIsNegative()
        {
            CreateEventStreamStub().GetMessage<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetMessage_Throws_IfIndexIsOutOfRange()
        {
            CreateEventStreamStub().GetMessage<object>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetMessage_Throws_IfEventIsNotOfExpectedType()
        {
            CreateEventStreamStub(string.Empty).GetMessage<int>(0);
        }

        [TestMethod]        
        public void GetMessage_ReturnsEvent_IfEventIsNotOfDerivedType()
        {
            Assert.AreSame(string.Empty, CreateEventStreamStub(string.Empty).GetMessage<object>(0));
        }

        [TestMethod]
        public void GetMessage_ReturnsEvent_IfEventIsNotOfExpectedType()
        {
            Assert.AreSame(string.Empty, CreateEventStreamStub(string.Empty).GetMessage<string>(0));
        }

        #endregion

        private static MessageStreamStub CreateEventStreamStub(params object[] events) =>
            new MessageStreamStub(CreateEventStream(events));

        private static MessageStream CreateEventStream(params object[] events) =>
            new MessageStream(events.Select(@event => MessageToDispatch.CreateEvent(@event, null)));
    }
}

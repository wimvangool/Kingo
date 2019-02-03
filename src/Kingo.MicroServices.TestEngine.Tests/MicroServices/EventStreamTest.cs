using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class EventStreamTest
    {
        #region [====== EventStreamStub ======]

        private sealed class EventStreamStub : EventStream
        {
            public EventStreamStub(EventStream stream) :
                base(stream) { }

            public new TEvent GetEvent<TEvent>(int index) =>
                base.GetEvent<TEvent>(index);
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfEventsIsNull()
        {
            new EventStream(null);
        }

        #endregion

        #region [====== AssertEvent ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void AssertEvent_Throws_IfIndexIsNegative()
        {
            EventStream.Empty.AssertEvent<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public void AssertEvent_Throws_IfIndexIsOutOfRange()
        {
            EventStream.Empty.AssertEvent<object>(0);
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

        #region [====== GetEvent ======]

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetEvent_Throws_IfIndexIsNegative()
        {
            CreateEventStreamStub().GetEvent<object>(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetEvent_Throws_IfIndexIsOutOfRange()
        {
            CreateEventStreamStub().GetEvent<object>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void GetEvent_Throws_IfEventIsNotOfExpectedType()
        {
            CreateEventStreamStub(string.Empty).GetEvent<int>(0);
        }

        [TestMethod]        
        public void GetEvent_ReturnsEvent_IfEventIsNotOfDerivedType()
        {
            Assert.AreSame(string.Empty, CreateEventStreamStub(string.Empty).GetEvent<object>(0));
        }

        [TestMethod]
        public void GetEvent_ReturnsEvent_IfEventIsNotOfExpectedType()
        {
            Assert.AreSame(string.Empty, CreateEventStreamStub(string.Empty).GetEvent<string>(0));
        }

        #endregion

        private static EventStreamStub CreateEventStreamStub(params object[] events) =>
            new EventStreamStub(CreateEventStream(events));

        private static EventStream CreateEventStream(params object[] events) =>
            new EventStream(events);
    }
}

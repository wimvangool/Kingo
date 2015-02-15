using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Modules
{
    [TestClass]
    public sealed class MemoryCacheControllerTest
    {
        #region [====== Nested Types ======]

        private sealed class RequestMessage : Message<RequestMessage>
        {
            internal readonly long Value;

            internal RequestMessage()
            {
                Value = Clock.Current.UtcDateAndTime().Ticks;
            }

            internal RequestMessage(long value)
            {
                Value = value;
            }

            public override RequestMessage Copy()
            {
                return new RequestMessage(Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, this))
                {
                    return true;
                }
                var other = obj as RequestMessage;
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                return Value == other.Value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        private sealed class ResponseMessage : Message<ResponseMessage>
        {
            internal readonly long Value;

            internal ResponseMessage(long value)
            {
                Value = value;
            }

            public override ResponseMessage Copy()
            {
                return new ResponseMessage(Value);
            }
        }

        private sealed class Query : Query<RequestMessage, ResponseMessage>
        {
            internal int InvocationCount
            {
                get;
                private set;
            }

            public override ResponseMessage Execute(RequestMessage message)
            {
                InvocationCount++;

                return new ResponseMessage(message.Value);
            }
        }

        private sealed class IdentityScope : IDisposable
        {
            private readonly IPrincipal _previousPrincipal;
            private bool _isDisposed;

            internal IdentityScope(string name)
            {
                _previousPrincipal = Thread.CurrentPrincipal;

                Thread.CurrentPrincipal = CreatePrincipal(name);
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }
                Thread.CurrentPrincipal = _previousPrincipal;

                _isDisposed = true;
            }

            private static IPrincipal CreatePrincipal(string name)
            {                
                return new GenericPrincipal(new GenericIdentity(name), new string[0]);
            }
        }

        #endregion
        
        private Query _query;
        private MemoryCacheController _controller;

        [TestInitialize]
        public void Setup()
        {            
            _query = new Query();
            _controller = new MemoryCacheController();
        }

        [TestCleanup]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [TestMethod]
        public void GetOrAddToSessionCache_ReturnsCachedResult_IfResultWasCachedForCurrentSession()
        {
            var requestMessage = CreateRequestMessage(new RequestMessage());
            var resultOne = _controller.GetOrAddToSessionCache(requestMessage, _query);
            var resultTwo = _controller.GetOrAddToSessionCache(requestMessage, _query);

            Assert.AreEqual(1, _query.InvocationCount);
            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreNotSame(resultOne, resultTwo);
            Assert.AreEqual(resultOne.Value, resultTwo.Value);
        }

        [TestMethod]
        public void GetOrAddToSessionCache_DoesNotReturnCachedResult_IfResultWasCachedForAnotherSession()
        {
            var requestMessage = CreateRequestMessage(new RequestMessage());
            var resultOne = _controller.GetOrAddToSessionCache(requestMessage, _query);
            
            using (new IdentityScope("TestUser"))
            {
                var resultTwo = _controller.GetOrAddToSessionCache(requestMessage, _query);
                var resultThree = _controller.GetOrAddToSessionCache(requestMessage, _query);

                Assert.AreEqual(2, _query.InvocationCount);                
                Assert.AreNotSame(resultOne, resultTwo);
                Assert.AreNotSame(resultTwo, resultThree);
                Assert.AreEqual(resultOne.Value, resultTwo.Value);
                Assert.AreEqual(resultTwo.Value, resultThree.Value);
            }
        }

        private static QueryRequestMessage<RequestMessage> CreateRequestMessage(RequestMessage message)
        {
            return new QueryRequestMessage<RequestMessage>(message, QueryExecutionOptions.Default, null, null);
        }
    }
}

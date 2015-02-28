using System.Security.Principal;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Caching
{
    [TestClass]
    public sealed class MemoryCacheModuleTest
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

        private RequestMessage _message;
        private Query _querySpy;
        private MemoryCacheModule _module;

        [TestInitialize]
        public void Setup()
        {
            _message = new RequestMessage(); ;
            _querySpy = new Query();
            _module = new MemoryCacheModule();
        }

        [TestCleanup]
        public void TearDown()
        {
            _module.Dispose();
        }

        private IQuery<ResponseMessage> CreateQuery()
        {
            return new QueryWrapper<RequestMessage, ResponseMessage>(_message, _querySpy, QueryExecutionOptions.Default);
        }
            
        [TestMethod]
        public void GetOrAddToSessionCache_ReturnsCachedResult_IfResultWasCachedForCurrentSession()
        {
            var query = CreateQuery();
            var resultOne = _module.GetOrAddToSessionCache(query, null, null);
            var resultTwo = _module.GetOrAddToSessionCache(query, null, null);

            Assert.AreEqual(1, _querySpy.InvocationCount);
            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreNotSame(resultOne, resultTwo);
            Assert.AreEqual(resultOne.Value, resultTwo.Value);
        }

        [TestMethod]
        public void GetOrAddToSessionCache_DoesNotReturnCachedResult_IfResultWasCachedForAnotherSession()
        {
            var query = CreateQuery();
            var resultOne = _module.GetOrAddToSessionCache(query, null, null);
            
            using (new IdentityScope("TestUser"))
            {
                var resultTwo = _module.GetOrAddToSessionCache(query, null, null);
                var resultThree = _module.GetOrAddToSessionCache(query, null, null);

                Assert.AreEqual(2, _querySpy.InvocationCount);                
                Assert.AreNotSame(resultOne, resultTwo);
                Assert.AreNotSame(resultTwo, resultThree);
                Assert.AreEqual(resultOne.Value, resultTwo.Value);
                Assert.AreEqual(resultTwo.Value, resultThree.Value);
            }
        }        
    }
}

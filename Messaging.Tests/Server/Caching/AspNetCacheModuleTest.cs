using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Transactions;
using System.Web;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server.Caching
{
    [TestClass]
    public sealed class AspNetCacheModuleTest
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
                return Equals(obj as RequestMessage);
            }

            public bool Equals(RequestMessage other)
            {
                if (ReferenceEquals(other, null))
                {
                    return false;
                }
                if (ReferenceEquals(other, this))
                {
                    return true;
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
            private int _invocationCount;
            private long _lastResult;

            internal void VerifyThatInvocationCountIs(int invocationCount)
            {
                Assert.AreEqual(invocationCount, _invocationCount);
            }

            internal void VerifyLastResultIs(ResponseMessage result)
            {
                Assert.AreEqual(result.Value, _lastResult);
            }

            public override ResponseMessage Execute(RequestMessage message)
            {
                _invocationCount++;
                _lastResult = message.Value;

                return new ResponseMessage(message.Value);
            }
        }

        private sealed class HttpContextScope : IDisposable
        {
            private readonly HttpContext _previousContext;
            private bool _isDisposed;

            internal HttpContextScope()
                : this(Guid.NewGuid().ToString("N")) { }

            private HttpContextScope(string sessionId)
            {
                _previousContext = HttpContext.Current;

                HttpContext.Current = CreateNewHttpContext(sessionId);
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }
                HttpContext.Current = _previousContext;

                _isDisposed = true;
            }

            private static readonly ConcurrentDictionary<string, HttpSessionState> _Sessions;

            static HttpContextScope()
            {
                _Sessions = new ConcurrentDictionary<string, HttpSessionState>();
            }

            private static HttpContext CreateNewHttpContext(string sessionId)
            {
                var httpRequest = new HttpRequest(null, @"http://testuri.org", null);
                var httpResponse = new HttpResponse(null);
                var httpContext = new HttpContext(httpRequest, httpResponse);

                AddApplicationStateTo(httpContext);
                AddSessionStateTo(httpContext, sessionId);

                return httpContext;
            }

            private static void AddApplicationStateTo(HttpContext context)
            {
                var applicationFactoryType = Type.GetType("System.Web.HttpApplicationFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                var applicationFactory = applicationFactoryType.GetField("_theApplicationFactory", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                var stateField = applicationFactoryType.GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);

                stateField.SetValue(applicationFactory, context.Application);
            }

            private static void AddSessionStateTo(HttpContext context, string sessionId)
            {
                context.Items["AspSession"] = GetOrAddSession(sessionId);
            }

            private static object GetOrAddSession(string sessionId)
            {
                return _Sessions.GetOrAdd(sessionId, id =>
                {
                    var sessionContainer = new HttpSessionStateContainer(
                        sessionId,
                        new SessionStateItemCollection(),
                        new HttpStaticObjectsCollection(),
                        10,
                        true,
                        HttpCookieMode.AutoDetect,
                        SessionStateMode.InProc,
                        false);

                    var httpSessionStateConstructor = typeof(HttpSessionState).GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null, CallingConventions.Standard,
                        new[] { typeof(HttpSessionStateContainer) },
                        null);

                    return httpSessionStateConstructor.Invoke(new object[] { sessionContainer }) as HttpSessionState;
                });
            }
        }

        private sealed class AspNetCacheProviderSpy : AspNetCacheModule
        {
            private readonly ManualResetEventSlim _evictionHandle;

            internal AspNetCacheProviderSpy()
            {
                _evictionHandle = new ManualResetEventSlim();
            }

            protected override void Dispose(bool disposing)
            {
                if (IsDisposed)
                {
                    return;
                }
                if (disposing)
                {
                    _evictionHandle.Dispose();
                }
                base.Dispose(disposing);
            }

            internal void WaitForCacheItemEviction(TimeSpan timeout, bool expectEviction = true)
            {
                if (expectEviction)
                {
                    if (_evictionHandle.Wait(timeout))
                    {
                        return;
                    }
                    Assert.Fail("Cache Item was not removed within the expected time interval ({0}).", timeout);
                }
                else if (_evictionHandle.Wait(timeout))
                {
                    Assert.Fail("Cache Item was expected not to be removed.");
                }
            }

            protected internal override void OnCacheEntryDeleted(CacheEntryDeletedEventArgs e)
            {
                _evictionHandle.Set();

                base.OnCacheEntryDeleted(e);
            }
        }

        #endregion

        private RequestMessage _message;
        private Query _querySpy;
        private AspNetCacheProviderSpy _provider;
        private HttpContextScope _httpContextScope;

        [TestInitialize]
        public void Setup()
        {
            _message = new RequestMessage();
            _querySpy = new Query();
            _provider = new AspNetCacheProviderSpy();
            _httpContextScope = new HttpContextScope();
        }

        [TestCleanup]
        public void TearDown()
        {
            _httpContextScope.Dispose();
            _provider.Dispose();
        }

        private IQuery<ResponseMessage> CreateQuery()
        {
            return new QueryWrapper<RequestMessage, ResponseMessage>(_message, _querySpy, QueryExecutionOptions.Default);
        }
            
        [TestMethod]
        public void GetOrAddToSessionCache_ReturnsCachedResult_IfResultWasCachedForCurrentSession()
        {
            var query = CreateQuery();
            var resultOne = _provider.GetOrAddToSessionCache(query, null, null);
            var resultTwo = _provider.GetOrAddToSessionCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(1);

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreNotSame(resultOne, resultTwo);
            Assert.AreEqual(resultOne.Value, resultTwo.Value);
        }

        [TestMethod]
        public void GetOrAddToSessionCache_DoesNotReturnCachedResult_IfResultWasCachedForAnotherSession()
        {
            var query = CreateQuery();
            var resultOne = _provider.GetOrAddToSessionCache(query, null, null);

            using (new HttpContextScope())
            {
                var resultTwo = _provider.GetOrAddToSessionCache(query, null, null);
                var resultThree = _provider.GetOrAddToSessionCache(query, null, null);

                _querySpy.VerifyThatInvocationCountIs(2);

                Assert.AreNotSame(resultOne, resultTwo);
                Assert.AreNotSame(resultTwo, resultThree);
                Assert.AreEqual(resultOne.Value, resultTwo.Value);
                Assert.AreEqual(resultTwo.Value, resultThree.Value);
            }
        }

        [TestMethod]
        public void HttpContext_AlwaysReturnsTheSameInstanceOfHttpApplicationState()
        {
            var applicationStateOne = HttpContext.Current.Application;
            var applicationStateTwo = HttpContext.Current.Application;

            Assert.AreSame(applicationStateOne, applicationStateTwo);
        }

        #region [====== ApplicationCache - CacheItem Invalidation and Removal ======]

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfAbsoluteExpirationIsSet()
        {
            var query = CreateQuery();

            var resultOne = _provider.GetOrAddToApplicationCache(query, TimeSpan.FromSeconds(5), null);

            _provider.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

            var resultTwo = _provider.GetOrAddToApplicationCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(2);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);            
        }

        [TestMethod]
        public void CacheItem_IsRemovedAfterSpecifiedPeriod_IfSlidingExpirationIsSet()
        {
            var query = CreateQuery();

            var resultOne = _provider.GetOrAddToApplicationCache(query, null, TimeSpan.FromSeconds(5));

            _provider.WaitForCacheItemEviction(TimeSpan.FromSeconds(30));

            var resultTwo = _provider.GetOrAddToApplicationCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(2);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);            
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndNoTransactionIsActive()
        {
            var query = CreateQuery();

            var resultOne = _provider.GetOrAddToApplicationCache(query, null, null);

            _provider.InvalidateIfRequired<RequestMessage>(message => true);
            _provider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

            var resultTwo = _provider.GetOrAddToApplicationCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(2);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);            
        }

        [TestMethod]
        public void CacheItem_IsRemoved_IfItemIsInvalidatedManuallyAndTransactionCommits()
        {
            var query = CreateQuery();

            var resultOne = _provider.GetOrAddToApplicationCache(query, null, null);

            using (var transactionScope = new TransactionScope())
            {
                _provider.InvalidateIfRequired<RequestMessage>(message => true);

                transactionScope.Complete();
            }
            _provider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10));

            var resultTwo = _provider.GetOrAddToApplicationCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(2);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);            
        }

        [TestMethod]
        public void CacheItem_IsNotRemoved_IfItemIsInvalidatedManuallyAndTransactionDoesNotCommit()
        {
            var query = CreateQuery();

            var resultOne = _provider.GetOrAddToApplicationCache(query, null, null);

            using (new TransactionScope())
            {
                _provider.InvalidateIfRequired<RequestMessage>(message => true);
            }
            _provider.WaitForCacheItemEviction(TimeSpan.FromSeconds(10), false);

            var resultTwo = _provider.GetOrAddToApplicationCache(query, null, null);

            _querySpy.VerifyThatInvocationCountIs(1);
            _querySpy.VerifyLastResultIs(resultOne);
            _querySpy.VerifyLastResultIs(resultTwo);            
        }       

        #endregion
    }
}

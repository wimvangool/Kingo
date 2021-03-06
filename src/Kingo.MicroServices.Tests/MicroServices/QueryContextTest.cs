﻿using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryContextTest
    {
        #region [====== Scoping ======]

        [TestMethod]
        public void Current_IsNull_IfCheckedOutsideAnyScope()
        {
            Assert.IsNull(QueryContext.Current);
        }

        [TestMethod]
        public void Current_IsNotNull_IfCheckedInsideScope()
        {
            using (CreateQueryContextScope())
            {
                Assert.IsNotNull(QueryContext.Current);
            }
        }

        [TestMethod]
        public void Current_IsUpdatedAsExpected_AsScopesAreCreatedAndDisposed()
        {
            Assert.IsNull(MicroProcessorContext.Current);

            using (CreateQueryContextScope())
            {
                var context = QueryContext.Current;

                Assert.IsNotNull(context);

                using (CreateQueryContextScope())
                {
                    Assert.IsNotNull(QueryContext.Current);
                    Assert.AreNotSame(context, QueryContext.Current);
                }

                Assert.AreSame(context, QueryContext.Current);
            }

            Assert.IsNull(MicroProcessorContext.Current);
        }

        #endregion

        #region [====== Principal ======]

        [TestMethod]
        public void Principal_AlwaysReferencesProcessorPrincipal_AsScopesAreCreatedAndDisposed()
        {
            using (CreateQueryContextScope())
            {
                Assert.AreSame(Thread.CurrentPrincipal, QueryContext.Current.Principal);

                using (CreateQueryContextScope())
                {
                    Assert.AreSame(Thread.CurrentPrincipal, QueryContext.Current.Principal);
                }

                Assert.AreSame(Thread.CurrentPrincipal, QueryContext.Current.Principal);
            }
        }

        #endregion

        #region [====== Token ======]

        [TestMethod]
        public void Token_IsNone_IfSpecifiedTokenIsNull()
        {
            using (CreateQueryContextScope())
            {
                Assert.AreEqual(CancellationToken.None, QueryContext.Current.Token);
            }
        }

        [TestMethod]
        public void Token_IsSpecifiedToken_IfSpecifiedTokenIsNotNull()
        {
            var token = new CancellationToken();

            using (CreateQueryContextScope(token))
            {
                Assert.AreEqual(token, QueryContext.Current.Token);
            }
        }

        #endregion

        #region [====== Operation ======]      

        [TestMethod]
        public void Operation_ContainsNoMessage_IfQueryHasNoInputMessage()
        {            
            using (CreateQueryContextScope())
            {
                Assert.AreEqual("[Query]", QueryContext.Current.ToString());

                var operation = QueryContext.Current.Operation;

                Assert.IsNotNull(operation);
                Assert.IsNull(operation.Message);
                Assert.IsNull(operation.MessageType);
                Assert.AreEqual(MicroProcessorOperationTypes.Query, operation.Type);
                Assert.AreEqual(1, operation.StackTrace().Count());                
            }
        }

        [TestMethod]
        public void Operation_ContainsExpectedMessage_IfQueryHasInputMessage()
        {
            var message = new object();

            using (CreateQueryContextScope(null, message))
            {
                Assert.AreEqual("[Query] Object", QueryContext.Current.ToString());

                var operation = QueryContext.Current.Operation;

                Assert.IsNotNull(operation);
                Assert.AreSame(message, operation.Message);
                Assert.AreEqual(typeof(object), operation.MessageType);
                Assert.AreEqual(MicroProcessorOperationTypes.Query, operation.Type);
                Assert.AreEqual(1, operation.StackTrace().Count());                
            }
        }        

        #endregion

        private static IDisposable CreateQueryContextScope(CancellationToken? token = null, object message = null) =>
            CreateQueryContextScope(CreateQueryContext(token, message));

        private static IDisposable CreateQueryContextScope(QueryContext context) =>
            MicroProcessorContext.CreateScope(context);

        private static QueryContext CreateQueryContext(CancellationToken? token, object message = null) =>
            new QueryContext(ServiceProvider.Default, Thread.CurrentPrincipal, token, message);
    }
}

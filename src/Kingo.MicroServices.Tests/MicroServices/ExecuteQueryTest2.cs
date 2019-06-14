using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class ExecuteQueryTest2 : MicroProcessorTest
    {
        #region [====== Null Parameters ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync<object, object>(null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryMessageIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(new QueryStub(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as Func<object, QueryOperationContext, object>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncMessageIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync((message, context) => message, null as object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncAsyncIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as Func<object, QueryOperationContext, Task<object>>, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncAsyncMessageIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync((message, context) => Task.FromResult(message), null as object);
        }

        #endregion

        #region [====== Return Value ======]

        [TestMethod]
        public async Task ExecuteQueryAsync_ReturnsExpectedResponse_IfQueryIsExecuted()
        {
            var request = new object();

            var result = await CreateProcessor().ExecuteQueryAsync((message, context) => message, request);

            Assert.IsNotNull(result);
            Assert.AreSame(request, result.Response);
        }

        #endregion

        #region [====== Context Verification ======]

        private sealed class QueryStub : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, QueryOperationContext context)
            {
                Assert.AreEqual(1, context.StackTrace.Count);
                AssertOperation(message, context.StackTrace.CurrentOperation);
                return Task.FromResult(message);
            }

            private void AssertOperation(object message, IAsyncMethodOperation operation)
            {
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, operation.Type);
                Assert.AreEqual(MicroProcessorOperationKinds.RootOperation, operation.Kind);

                Assert.IsNotNull(operation.Message);
                Assert.AreSame(message, operation.Message.Instance);
                Assert.AreEqual(MessageKind.Request, operation.Message.Kind);

                Assert.IsNotNull(operation.Method.MessageParameter);
                Assert.AreSame(typeof(object), operation.Method.MessageParameter.Type);
                Assert.AreSame(typeof(QueryOperationContext), operation.Method.ContextParameter.Type);

                AssertComponent(operation.Method.Component as Query);
            }

            private void AssertComponent(Query query)
            {
                Assert.IsNotNull(query);
                Assert.AreSame(GetType(), query.Type);
                Assert.AreEqual(1, query.Interfaces.Count);
                Assert.AreSame(typeof(IQuery<object, object>), query.Interfaces.First().Type);
            }
        }

        [TestMethod]
        public async Task ExecuteQueryAsync_ProducesExpectedStackTrace_IfQueryIsExecuted()
        {
            await CreateProcessor().ExecuteQueryAsync(new QueryStub(), new object());
        }

        #endregion

        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationIsCancelledWithTheSpecifiedToken()
        {
            var tokenSource = new CancellationTokenSource();

            try
            {
                await CreateProcessor().ExecuteQueryAsync((message, context) =>
                {
                    tokenSource.Cancel();
                    return message;
                }, new object(), tokenSource.Token);
            }
            catch (OperationCanceledException exception)
            {
                Assert.AreEqual(tokenSource.Token, exception.CancellationToken);
                throw;
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationIsCancelledWithSomeOtherToken()
        {
            var exceptionToThrow = new OperationCanceledException();
            var tokenSource = new CancellationTokenSource();

            try
            {
                await CreateProcessor().ExecuteQueryAsync<object, object>((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object(), tokenSource.Token);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
            finally
            {
                tokenSource.Dispose();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException), AllowDerivedTypes = true)]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsMessageHandlerOperationException()
        {
            var exceptionToThrow = new BusinessRuleException();

            try
            {
                await CreateProcessor().ExecuteQueryAsync<object, object>((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException();

            try
            {
                await CreateProcessor().ExecuteQueryAsync<object, object>((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (BadRequestException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsInternalServerErrorException()
        {
            var exceptionToThrow = new InternalServerErrorException();

            try
            {
                await CreateProcessor().ExecuteQueryAsync<object, object>((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsRandomException()
        {
            var exceptionToThrow = new Exception();

            try
            {
                await CreateProcessor().ExecuteQueryAsync<object, object>((message, context) =>
                {
                    throw exceptionToThrow;
                }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                throw;
            }
        }

        #endregion
    }
}

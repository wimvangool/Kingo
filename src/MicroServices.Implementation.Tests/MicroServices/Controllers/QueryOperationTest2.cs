using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class QueryOperationTest2 : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Null Parameters ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as IQuery<object, object>, new object());
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
            await CreateProcessor().ExecuteQueryAsync(null as Func<object, IQueryOperationContext, object>, new object());
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
            await CreateProcessor().ExecuteQueryAsync(null as Func<object, IQueryOperationContext, Task<object>>, new object());
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
            Assert.AreSame(request, result.Input.Content);
            Assert.AreSame(request, result.Output.Content);

            Assert.AreEqual(36, result.Input.Id.Length);
            Assert.IsNull(result.Input.CorrelationId);

            Assert.AreEqual(36, result.Output.Id.Length);
            Assert.AreEqual(result.Input.Id, result.Output.CorrelationId);
        }

        #endregion

        #region [====== Context Verification ======]

        private sealed class QueryStub : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, IQueryOperationContext context)
            {
                Assert.AreEqual(1, context.StackTrace.Count);
                AssertOperation(message, context.StackTrace.CurrentOperation);
                return Task.FromResult(message);
            }

            private void AssertOperation(object message, IAsyncMethodOperation operation)
            {
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, operation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.RootOperation, operation.Kind);

                Assert.IsNotNull(operation.Message);
                Assert.AreSame(message, operation.Message.Content);
                Assert.AreEqual(MessageKind.Request, operation.Message.Kind);

                Assert.IsNotNull(operation.Method.MessageParameterInfo);
                Assert.AreSame(typeof(object), operation.Method.MessageParameterInfo.ParameterType);
                Assert.AreSame(typeof(IQueryOperationContext), operation.Method.ContextParameterInfo.ParameterType);

                AssertComponentType(operation.Method.ComponentType);
            }

            private void AssertComponentType(Type queryType)
            {
                Assert.IsNotNull(queryType);
                Assert.AreSame(GetType(), queryType);
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
        [ExpectedException(typeof(GatewayTimeoutException), AllowDerivedTypes = true)]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationIsCancelledWithTheSpecifiedToken()
        {
            var tokenSource = new CancellationTokenSource();

            try
            {
                await CreateProcessor().ExecuteQueryAsync((message, context) =>
                {
                    tokenSource.Cancel();
                    return message;
                }, new object(), MessageHeader.Unspecified, tokenSource.Token);
            }
            catch (GatewayTimeoutException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(OperationCanceledException));
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
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
                }, new object(), MessageHeader.Unspecified, tokenSource.Token);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
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
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsBadRequestException()
        {
            var exceptionToThrow = new BadRequestException(null, null);

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
                Assert.AreEqual(0, exception.OperationStackTrace.Count);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationThrowsInternalServerErrorException()
        {
            var exceptionToThrow = new InternalServerErrorException(null, null);

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
                Assert.AreEqual(0, exception.OperationStackTrace.Count);
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
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsInstanceOfType(exception.OperationStackTrace.RootOperation.Message.Content, typeof(object));
                Assert.IsInstanceOfType(exception.OperationStackTrace.CurrentOperation.Message.Content, typeof(object));
                throw;
            }
        }

        #endregion
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class QueryOperationTest1 : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Null Parameters ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as IQuery<object>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as Func<IQueryOperationContext, object>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task ExecuteQueryAsync_Throws_IfQueryFuncAsyncIsNull()
        {
            await CreateProcessor().ExecuteQueryAsync(null as Func<IQueryOperationContext, Task<object>>);
        }

        #endregion

        #region [====== Return Value ======]

        [TestMethod]
        public async Task ExecuteQueryAsync_ReturnsExpectedResponse_IfQueryIsExecuted()
        {
            var response = new object();
            var result = await CreateProcessor().ExecuteQueryAsync(context => response);

            Assert.IsNotNull(result);
            Assert.AreSame(response, result.Output.Content);

            Assert.AreEqual(36, result.Output.Id.Length);
            Assert.IsNull(result.Output.CorrelationId);
        }

        #endregion

        #region [====== Context Verification ======]

        private sealed class QueryStub : IQuery<object>
        {
            public Task<object> ExecuteAsync(IQueryOperationContext context)
            {
                Assert.AreEqual(1, context.StackTrace.Count);
                AssertOperation(context.StackTrace.CurrentOperation);
                return Task.FromResult(new object());
            }

            private void AssertOperation(IAsyncMethodOperation operation)
            {
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, operation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.RootOperation, operation.Kind);

                Assert.IsNull(operation.Message);
                Assert.IsNull(operation.Method.MessageParameterInfo);
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
            await CreateProcessor().ExecuteQueryAsync(new QueryStub());
        }

        #endregion

        #region [====== Exception Handling ======]

        [TestMethod]
        [ExpectedException(typeof(GatewayTimeoutException))]
        public async Task ExecuteQueryAsync_ThrowsExpectedException_IfOperationIsCancelledWithTheSpecifiedToken()
        {
            var tokenSource = new CancellationTokenSource();

            try
            {
                await CreateProcessor().ExecuteQueryAsync(context =>
                {
                    tokenSource.Cancel();
                    return new object();
                }, tokenSource.Token);
            }
            catch (GatewayTimeoutException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(OperationCanceledException));
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsNull(exception.OperationStackTrace.RootOperation.Message);
                Assert.IsNull(exception.OperationStackTrace.CurrentOperation.Message);
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
                await CreateProcessor().ExecuteQueryAsync<object>(context =>
                {
                    throw exceptionToThrow;
                }, tokenSource.Token);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsNull(exception.OperationStackTrace.RootOperation.Message);
                Assert.IsNull(exception.OperationStackTrace.CurrentOperation.Message);
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
                await CreateProcessor().ExecuteQueryAsync<object>(context =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsNull(exception.OperationStackTrace.RootOperation.Message);
                Assert.IsNull(exception.OperationStackTrace.CurrentOperation.Message);
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
                await CreateProcessor().ExecuteQueryAsync<object>(context =>
                {
                    throw exceptionToThrow;
                });
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
                await CreateProcessor().ExecuteQueryAsync<object>(context =>
                {
                    throw exceptionToThrow;
                });
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
                await CreateProcessor().ExecuteQueryAsync<object>(context =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreSame(exceptionToThrow, exception.InnerException);
                Assert.AreEqual(1, exception.OperationStackTrace.Count);
                Assert.IsNull(exception.OperationStackTrace.RootOperation.Message);
                Assert.IsNull(exception.OperationStackTrace.CurrentOperation.Message);
                throw;
            }
        }

        #endregion
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class ExecuteInternalQueryTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== PublishResultFunction ======]

        private sealed class PublishResultFunction : IMessageHandler<int>
        {
            private readonly IGetProductQuery _getProductQuery;

            public PublishResultFunction(IGetProductQuery getProductQuery)
            {
                _getProductQuery = getProductQuery;
            }

            public async Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                Assert.AreEqual(1, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.RootOperation, context.StackTrace.CurrentOperation.Kind);

                context.MessageBus.Publish(await _getProductQuery.ExecuteAsync(message, context));
            }
        }

        #endregion

        #region [====== GetProductQuery ======]

        private interface IGetProductQuery : IQuery<int, int>
        {
            Task<int> ExecuteAsync(int message, MicroProcessorOperationContext context) =>
                context.QueryProcessor.ExecuteQueryAsync(this, message);
        }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IGetProductQuery))]
        private sealed class GetProductQuery : IGetProductQuery
        {
            private readonly IGetConstantValueQuery _getConstantValueQuery;

            public GetProductQuery(IGetConstantValueQuery getConstantValueQuery)
            {
                _getConstantValueQuery = getConstantValueQuery;
            }

            public async Task<int> ExecuteAsync(int message, QueryOperationContext context)
            {
                Assert.AreEqual(2, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);

                return message * await _getConstantValueQuery.ExecuteAsync(context);
            }
        }

        #endregion

        #region [====== GetConstantValueQuery ======]

        private interface IGetConstantValueQuery : IQuery<int>
        {
            Task<int> ExecuteAsync(MicroProcessorOperationContext context) =>
                context.QueryProcessor.ExecuteQueryAsync(this);
        }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IGetConstantValueQuery))]
        private sealed class GetConstantValueQuery : IGetConstantValueQuery
        {
            public Task<int> ExecuteAsync(QueryOperationContext context)
            {
                Assert.AreEqual(3, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);

                return Task.FromResult(_Value);
            }
        }

        #endregion

        private static readonly int _Value = DateTimeOffset.UtcNow.Second;

        [TestMethod]
        public async Task ExecuteCommandAsync_ExecutesInternalQueriesAsExpected()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<PublishResultFunction>();
            });
            Processor.ConfigureQueries(queries =>
            {
                queries.Add<GetProductQuery>();
                queries.Add<GetConstantValueQuery>();
            });

            var processor = CreateProcessor();
            var messageHandler = processor.ServiceProvider.GetRequiredService<PublishResultFunction>();
            var result = await processor.ExecuteCommandAsync(messageHandler, 3);

            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Output.Count);
            Assert.AreEqual(3 * _Value, result.Output[0].Content);
        }
    }
}

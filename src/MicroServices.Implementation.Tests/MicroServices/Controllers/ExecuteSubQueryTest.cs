using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class ExecuteSubQueryTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MessageHandler & Query Types ======]

        private sealed class CommandHandler : IMessageHandler<int>
        {
            private readonly IConstantProvider _constantProvider;
            private readonly IMultiplier _multiplier;

            public CommandHandler(IConstantProvider constantProvider, IMultiplier multiplier)
            {
                _constantProvider = constantProvider;
                _multiplier = multiplier;
            }

            public async Task HandleAsync(int message, IMessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(await _constantProvider.GetConstantAsync(context));
                context.MessageBus.Publish(await _multiplier.MultiplyByTwoAsync(message, context));
            }                
        }

        private sealed class QueryWithSubQueries : IQuery<int, int>
        {
            private readonly IConstantProvider _constantProvider;
            private readonly IMultiplier _multiplier;

            public QueryWithSubQueries(IConstantProvider constantProvider, IMultiplier multiplier)
            {
                _constantProvider = constantProvider;
                _multiplier = multiplier;
            }

            public async Task<int> ExecuteAsync(int message, IQueryOperationContext context)
            {
                var a = await _constantProvider.GetConstantAsync(context);
                var b = await _multiplier.MultiplyByTwoAsync(a, context);
                return b + message;
            }                
        }

        private interface IConstantProvider
        {
            Task<int> GetConstantAsync(IMicroProcessorOperationContext context);
        }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IConstantProvider))]
        private sealed class ConstantProviderQuery : IConstantProvider, IQuery<int>
        {
            Task<int> IConstantProvider.GetConstantAsync(IMicroProcessorOperationContext context) =>
                context.QueryProcessor.ExecuteQueryAsync(this);

            public Task<int> ExecuteAsync(IQueryOperationContext context)
            {
                Assert.AreEqual(2, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);
                return Task.FromResult(10);
            }
        }

        private interface IMultiplier
        {
            Task<int> MultiplyByTwoAsync(int value, IMicroProcessorOperationContext context);
        }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IMultiplier))]
        private sealed class MultiplierQuery : IMultiplier, IQuery<int, int>
        {
            Task<int> IMultiplier.MultiplyByTwoAsync(int value, IMicroProcessorOperationContext context) =>
                context.QueryProcessor.ExecuteQueryAsync(this, value);

            public Task<int> ExecuteAsync(int message, IQueryOperationContext context)
            {
                Assert.AreEqual(2, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.QueryOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);
                return Task.FromResult(2 * message);
            }                
        }

        #endregion

        [TestMethod]
        public async Task ExecuteCommandAsync_ExecutesSubQueryAsExpected()
        {
            ProcessorBuilder.Components.AddMessageHandler<CommandHandler>();
            ProcessorBuilder.Components.AddQuery<ConstantProviderQuery>();
            ProcessorBuilder.Components.AddQuery<MultiplierQuery>();

            var processor = CreateProcessor();
            var messageHandler = processor.ServiceProvider.GetRequiredService<CommandHandler>();
            var result = await processor.ExecuteCommandAsync(messageHandler, 3);

            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(2, result.Messages.Count);
            Assert.AreEqual(10, result.Messages[0].Content);
            Assert.AreEqual(6, result.Messages[1].Content);
        }

        [TestMethod]
        public async Task ExecuteQueryAsync_ExecutesSubQueryAsExpected()
        {
            ProcessorBuilder.Components.AddQuery<QueryWithSubQueries>();
            ProcessorBuilder.Components.AddQuery<ConstantProviderQuery>();
            ProcessorBuilder.Components.AddQuery<MultiplierQuery>();

            var processor = CreateProcessor();
            var query = processor.ServiceProvider.GetRequiredService<QueryWithSubQueries>();
            var result = await processor.ExecuteQueryAsync(query, 3);

            Assert.AreEqual(23, result.Response);            
        }
    }
}

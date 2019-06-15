using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class AddComponentsTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Nested Types ======]        

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ReadModel : IMessageHandler<object>, IQuery<object>
        {
            public Task HandleAsync(object message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;

            public Task<object> ExecuteAsync(QueryOperationContext context) =>
                Task.FromResult(new object());
        }

        #endregion        

        #region [====== AddComponents ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddComponents_Throws_IfServiceFactoryIsNull()
        {
            ProcessorBuilder.Components.AddComponents(null);
        }

        #endregion

        #region [====== AddReadModels ======]

        [TestMethod]
        public void AddReadModel_AddsExpectedReadModel_IfTypeIsBothMessageHandlerAndQuery()
        {
            ProcessorBuilder.Components.AddType<ReadModel>();
            ProcessorBuilder.Components.AddMessageHandlers();
            ProcessorBuilder.Components.AddQueries();

            var provider = BuildServiceProvider();

            var messageHandler = provider.GetRequiredService<IMessageHandler<object>>();
            var query = provider.GetRequiredService<IQuery<object>>();
            var readModel = provider.GetRequiredService<ReadModel>();

            Assert.AreSame(messageHandler, query);
            Assert.AreSame(query, readModel);
        }

        #endregion        
    }
}

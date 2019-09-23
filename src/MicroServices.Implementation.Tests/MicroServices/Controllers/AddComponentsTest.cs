using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddComponentsTest : MicroProcessorTest<MicroProcessor>
    {              
        #region [====== AddComponents ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddComponents_Throws_IfServiceFactoryIsNull()
        {
            ProcessorBuilder.Components.AddComponents(null);
        }        

        #endregion

        #region [====== AddReadModels ======]

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ReadModel : IMessageHandler<object>, IQuery<object>
        {
            public Task HandleAsync(object message, IMessageHandlerOperationContext context) =>
                Task.CompletedTask;

            public Task<object> ExecuteAsync(IQueryOperationContext context) =>
                Task.FromResult(new object());
        }

        [TestMethod]
        public void AddReadModel_AddsExpectedReadModel_IfTypeIsBothMessageHandlerAndQuery()
        {
            ProcessorBuilder.Components.AddToSearchSet<ReadModel>();
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

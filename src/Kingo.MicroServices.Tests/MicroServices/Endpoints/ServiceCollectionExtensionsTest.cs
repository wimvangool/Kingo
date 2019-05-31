using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class ServiceCollectionExtensionsTest
    {
        #region [====== ScopedMessageHandler ======]

        [MessageHandler(ServiceLifetime.Scoped, HandleInputMessages = true, HandleOutputMessages = true)]
        private sealed class ScopedMessageHandler : IMessageHandler<object>
        {            
            public const int TotalInvocationCount = 5;
            private int _invocationCount;           

            public Task HandleAsync(object message, MessageHandlerContext context)
            {
                if (Interlocked.Increment(ref _invocationCount) < TotalInvocationCount)
                {
                    context.EventBus.Publish(message);                    
                }
                return Task.CompletedTask;
            }
        }

        #endregion

        private readonly IServiceCollection _services;

        public ServiceCollectionExtensionsTest()
        {
            _services = new ServiceCollection();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMicroProcessor_Throws_IfServiceCollectionIsNull()
        {
            ServiceCollectionExtensions.AddMicroProcessor(null);
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersDefaultProcessor_IfNothingIsConfigured()
        {
            _services.AddMicroProcessor();

            var processor = ResolveProcessor<MicroProcessor>();
            var result = await processor.HandleAsync(new object(), (message, context) =>
            {
                Assert.AreSame(processor, ResolveProcessor(context.ServiceProvider));
            });

            Assert.AreEqual(1, result.MessageHandlerCount);
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersCustomProcessor_IfTypeParameterIsUsed()
        {
            _services.AddMicroProcessor<MicroProcessorStub>();

            var processor = ResolveProcessor<MicroProcessorStub>();
            var result = await processor.HandleAsync(new object(), (message, context) =>
            {
                Assert.AreSame(processor, ResolveProcessor(context.ServiceProvider));
            });

            Assert.AreEqual(1, result.MessageHandlerCount);
        }        

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerInstances_IfOneOrMoreMessageHandlerInstancesAreAdded()
        {            
            var @event = new object();

            _services.AddMicroProcessor<MicroProcessorStub>(processor =>
            {                
                processor.Components.AddMessageHandler<object>((message, context) =>
                {
                    context.EventBus.Publish(@event);
                });
            });

            var result = await ResolveProcessor().HandleAsync(new object());

            Assert.AreEqual(1, result.MessageHandlerCount);

            result.Events.AssertCountIs(1);
            result.Events.AssertAreSame(0, @event);
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerTypes_IfOneOrMoreMessageHandlerTypesAreAdded()
        {            
            _services.AddMicroProcessor<MicroProcessorStub>(processor =>
            {                
                processor.Components.AddMessageHandler<ScopedMessageHandler>();
            });

            Assert.AreEqual(ScopedMessageHandler.TotalInvocationCount, (await ResolveProcessor().HandleAsync(new object())).MessageHandlerCount);            
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedFilters_IfOneOrMoreFiltersAreAdded()
        {                        
            _services.AddMicroProcessor<MicroProcessorStub>(processor =>
            {                
                processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ExceptionHandlingStage));
                processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.AuthorizationStage));
                processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ValidationStage));
                processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ProcessingStage));
            });

            var result = await ResolveProcessor().HandleAsync(new object(), (message, context) => { });

            Assert.AreEqual(1, result.MessageHandlerCount);

            result.Events.AssertCountIs(4);
            result.Events.AssertAreEqual(0, MicroProcessorFilterStage.ExceptionHandlingStage);
            result.Events.AssertAreEqual(1, MicroProcessorFilterStage.AuthorizationStage);
            result.Events.AssertAreEqual(2, MicroProcessorFilterStage.ValidationStage);
            result.Events.AssertAreEqual(3, MicroProcessorFilterStage.ProcessingStage);
        }

        private TProcessor ResolveProcessor<TProcessor>() where TProcessor : IMicroProcessor =>
            (TProcessor) ResolveProcessor();

        private IMicroProcessor ResolveProcessor() =>
            ResolveProcessor(_services.BuildServiceProvider());

        private static IMicroProcessor ResolveProcessor(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<IMicroProcessor>();
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
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

            Assert.AreEqual(1, await processor.HandleAsync(new object(), (message, context) =>
            {
                Assert.AreSame(processor, ResolveProcessor(context.ServiceProvider));
            }));
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersCustomProcessor_IfTypeParameterIsUsed()
        {
            _services.AddMicroProcessor<MicroProcessorStub>();

            var processor = ResolveProcessor<MicroProcessorStub>();

            Assert.AreEqual(1, await processor.HandleAsync(new object(), (message, context) =>
            {
                Assert.AreSame(processor, ResolveProcessor(context.ServiceProvider));
            }));
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedServiceBusInstances_IfOneOrMoreServiceBusInstancesAreAdded()
        {
            var busA = new MicroServiceBusStub();
            var busB = new MicroServiceBusStub();
            var @event = new object();

            _services.AddMicroProcessor<MicroProcessorStub>(builder =>
            {
                builder.ServiceBus.Add(busA);
                builder.ServiceBus.Add(busB);
            });

            Assert.AreEqual(1, await ResolveProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(@event);
            }));

            busA.AssertEventCountIs(1);
            busA.AssertAreSame(0, @event);            

            busB.AssertEventCountIs(1);
            busB.AssertAreSame(0, @event);            
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerInstances_IfOneOrMoreMessageHandlerInstancesAreAdded()
        {
            var bus = new MicroServiceBusStub();
            var @event = new object();

            _services.AddMicroProcessor<MicroProcessorStub>(builder =>
            {
                builder.ServiceBus.Add(bus);                
                builder.MessageHandlers.Add<object>((message, context) =>
                {
                    context.EventBus.Publish(@event);
                });
            });

            Assert.AreEqual(1, await ResolveProcessor().HandleAsync(new object()));

            bus.AssertEventCountIs(1);
            bus.AssertAreSame(0, @event);
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerTypes_IfOneOrMoreMessageHandlerTypesAreAdded()
        {            
            _services.AddMicroProcessor<MicroProcessorStub>(builder =>
            {                
                builder.MessageHandlers.Add<ScopedMessageHandler>();
            });

            Assert.AreEqual(ScopedMessageHandler.TotalInvocationCount, await ResolveProcessor().HandleAsync(new object()));            
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedFilters_IfOneOrMoreFiltersAreAdded()
        {            
            var bus = new MicroServiceBusStub();

            _services.AddMicroProcessor<MicroProcessorStub>(builder =>
            {
                builder.ServiceBus.Add(bus);
                builder.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ExceptionHandlingStage));
                builder.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.AuthorizationStage));
                builder.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ValidationStage));
                builder.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ProcessingStage));
            });

            Assert.AreEqual(1, await ResolveProcessor().HandleAsync(new object(), (message, context) => {}));

            bus.AssertEventCountIs(4);
            bus.AssertAreEqual(0, MicroProcessorFilterStage.ExceptionHandlingStage);
            bus.AssertAreEqual(1, MicroProcessorFilterStage.AuthorizationStage);
            bus.AssertAreEqual(2, MicroProcessorFilterStage.ValidationStage);
            bus.AssertAreEqual(3, MicroProcessorFilterStage.ProcessingStage);
        }

        private TProcessor ResolveProcessor<TProcessor>() where TProcessor : IMicroProcessor =>
            (TProcessor) ResolveProcessor();

        private IMicroProcessor ResolveProcessor() =>
            ResolveProcessor(_services.BuildServiceProvider());

        private static IMicroProcessor ResolveProcessor(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<IMicroProcessor>();
    }
}

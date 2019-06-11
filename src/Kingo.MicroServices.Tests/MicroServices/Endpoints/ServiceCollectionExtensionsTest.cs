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
        #region [====== MicroProcessorStub ======]

        private sealed class MicroProcessorStub : MicroProcessor
        {
            public MicroProcessorStub(IServiceProvider serviceProvider) :
                base(serviceProvider) { }
        }

        #endregion

        #region [====== ScopedEventHandler ======]

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        [MessageHandler(HandlesExternalMessages = false, HandlesInternalMessages = true)]
        private sealed class ScopedEventHandler : IMessageHandler<object>
        {            
            public const int TotalInvocationCount = 5;
            private int _invocationCount;           

            public Task HandleAsync(object message, MessageHandlerOperationContext context)
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
        public void AddMicroProcessor_RegistersDefaultProcessor_IfNothingIsConfigured()
        {
            _services.AddMicroProcessor();

            Assert.IsInstanceOfType(ResolveProcessor(), typeof(MicroProcessor));
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersCustomProcessor_IfTypeParameterIsUsed()
        {
            _services.AddMicroProcessor<MicroProcessorStub>();

            Assert.IsInstanceOfType(ResolveProcessor(), typeof(MicroProcessorStub));
        }        

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerInstances_IfOneOrMoreMessageHandlerInstancesAreAdded()
        {            
            var @event = new object();

            _services.AddMicroProcessor<MicroProcessorStub>(processor =>
            {                
                processor.Components.AddMessageHandler<int>((message, context) =>
                {
                    context.EventBus.Publish(@event);
                    context.EventBus.Publish(@event);
                }, false, true);
            });

            var result = await ResolveProcessor().ExecuteAsync((message, context) =>
            {
                context.EventBus.Publish(DateTimeOffset.UtcNow.Second);
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);

            result.Events.AssertCountIs(3);
            result.Events.AssertAreSame(1, @event);
            result.Events.AssertAreSame(2, @event);
        }

        [TestMethod]
        public async Task AddMicroProcessor_RegistersAddedMessageHandlerTypes_IfOneOrMoreMessageHandlerTypesAreAdded()
        {            
            _services.AddMicroProcessor<MicroProcessorStub>(processor =>
            {                
                processor.Components.AddType<ScopedEventHandler>();
                processor.Components.AddMessageHandlers();                
            });

            var result = await ResolveProcessor().ExecuteAsync((message, context) =>
            {
                context.EventBus.Publish(message);
            }, new object());

            Assert.AreEqual(ScopedEventHandler.TotalInvocationCount + 1, result.MessageHandlerCount);            
        }

        //[TestMethod]
        //public async Task AddMicroProcessor_RegistersAddedFilters_IfOneOrMoreFiltersAreAdded()
        //{                        
        //    _services.AddMicroProcessor<MicroProcessorStub>(processor =>
        //    {                
        //        processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ExceptionHandlingStage));
        //        processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.AuthorizationStage));
        //        processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ValidationStage));
        //        processor.Pipeline.Add(new MicroProcessorFilterStub(MicroProcessorFilterStage.ProcessingStage));
        //    });

        //    var result = await ResolveProcessor().ExecuteAsync((message, context) => { }, new object());

        //    Assert.AreEqual(1, result.MessageHandlerCount);

        //    result.Events.AssertCountIs(4);
        //    result.Events.AssertAreEqual(0, MicroProcessorFilterStage.ExceptionHandlingStage);
        //    result.Events.AssertAreEqual(1, MicroProcessorFilterStage.AuthorizationStage);
        //    result.Events.AssertAreEqual(2, MicroProcessorFilterStage.ValidationStage);
        //    result.Events.AssertAreEqual(3, MicroProcessorFilterStage.ProcessingStage);
        //}

        private IMicroProcessor ResolveProcessor() =>
            _services.BuildServiceProvider().GetRequiredService<IMicroProcessor>();        
    }
}

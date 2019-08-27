using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddMicroServiceBusTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MicroServiceBus Types ======]

        private abstract class AbstractServiceBus : IMicroServiceBus
        {
            public abstract Task PublishAsync(IEnumerable<object> events);

            public abstract Task PublishAsync(object @event);
        }

        private sealed class GenericServiceBus<T> : IMicroServiceBus
        {
            public Task PublishAsync(IEnumerable<object> events) =>
                Task.CompletedTask;

            public Task PublishAsync(object @event) =>
                Task.CompletedTask;
        }

        private abstract class MicroServiceBusBase : MicroServiceBus
        {
            private readonly IInstanceCollector _instances;

            protected MicroServiceBusBase(IInstanceCollector instances)
            {
                _instances = instances;
            }

            public override Task PublishAsync(IEnumerable<object> messages)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }

            public override Task PublishAsync(object message)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }
        }

        [MicroProcessorComponent]
        private sealed class TransientServiceBus : MicroServiceBusBase
        {
            public TransientServiceBus(IInstanceCollector instances) :
                base(instances) { }            
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedServiceBus : MicroServiceBusBase
        {
            public ScopedServiceBus(IInstanceCollector instances) :
                base(instances) { }
        }
        
        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonServiceBus : MicroServiceBusBase
        {
            public SingletonServiceBus(IInstanceCollector instances) :
                base(instances) { }
        }       

        #endregion

        #region [====== AddMicroServiceBus ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMicroServiceBus_Throws_IfTypeIsNull()
        {
            ProcessorBuilder.Components.AddMicroServiceBus(null);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeDoesNotImplementMicroServiceBusInterface()
        {
            ProcessorBuilder.Components.AddMicroServiceBus(typeof(object));

            var provider = BuildServiceProvider();
            var serviceBus = provider.GetRequiredService<IMicroServiceBus>();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            await serviceBus.PublishAsync(new object());

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsAbstract()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<AbstractServiceBus>();

            var provider = BuildServiceProvider();
            var serviceBus = provider.GetRequiredService<IMicroServiceBus>();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            await serviceBus.PublishAsync(new object());

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsGeneric()
        {
            ProcessorBuilder.Components.AddMicroServiceBus(typeof(GenericServiceBus<>));

            var provider = BuildServiceProvider();
            var serviceBus = provider.GetRequiredService<IMicroServiceBus>();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            await serviceBus.PublishAsync(new object());

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsTransientServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(2);

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsScopedServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<ScopedServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsSingletonServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<SingletonServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_BuildsExpectedServiceBus_IfMultipleServiceBusesAreAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<ScopedServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<SingletonServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(4);

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(7);
        }

        private static Task PublishMessageAsync(IMicroProcessor processor) =>
            processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

        #endregion

        #region [====== AddMicroServiceBuses ======]

        [TestMethod]
        public async Task AddMicroServiceBuses_DoesNothing_IfNoTypesWereAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBuses();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await PublishMessageAsync(processor);

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBuses_BuildsExpectedServiceBus_IfMultipleServiceBusesWereAddedAsType()
        {
            ProcessorBuilder.Components.AddToSearchSet<TransientServiceBus>();
            ProcessorBuilder.Components.AddToSearchSet<ScopedServiceBus>();
            ProcessorBuilder.Components.AddToSearchSet<SingletonServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBuses();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(4);

            using (processor.ServiceProvider.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(7);
        }

        #endregion
    }
}

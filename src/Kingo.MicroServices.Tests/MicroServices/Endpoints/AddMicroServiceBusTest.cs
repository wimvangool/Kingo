using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class AddMicroServiceBusTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MicroServiceBus Types ======]

        private abstract class AbstractServiceBus : IMicroServiceBus
        {
            public abstract Task PublishAsync(IEnumerable<object> messages);

            public abstract Task PublishAsync(object message);
        }

        private sealed class GenericServiceBus<T> : IMicroServiceBus
        {
            public Task PublishAsync(IEnumerable<object> messages) =>
                Task.CompletedTask;

            public Task PublishAsync(object message) =>
                Task.CompletedTask;
        }

        private abstract class MicroServiceBus : IMicroServiceBus
        {
            private readonly IInstanceCollector _instances;

            protected MicroServiceBus(IInstanceCollector instances)
            {
                _instances = instances;
            }

            public Task PublishAsync(IEnumerable<object> messages)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }

            public Task PublishAsync(object message)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }
        }

        private sealed class TransientServiceBus : MicroServiceBus
        {
            public TransientServiceBus(IInstanceCollector instances) :
                base(instances) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedServiceBus : MicroServiceBus
        {
            public ScopedServiceBus(IInstanceCollector instances) :
                base(instances) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Singleton)]
        private sealed class SingletonServiceBus : MicroServiceBus
        {
            public SingletonServiceBus(IInstanceCollector instances) :
                base(instances) { }
        }

        private sealed class HostedServiceBus : MicroServiceBus, IHostedService
        {
            public HostedServiceBus(IInstanceCollector instances) :
                base(instances) { }

            public Task StartAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            public Task StopAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;            
        }

        #endregion

        #region [====== AddMicroServiceBus (Type) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMicroServiceBus_Throws_IfTypeIsNull()
        {
            ProcessorBuilder.Components.AddMicroServiceBus(null);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeDoesNotImplementMicroServiceBusInterface()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<object>();

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

            using (processor.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(2);

            using (processor.CreateScope())
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

            using (processor.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.CreateScope())
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

            using (processor.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(1);
        }        

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusAsHostedService_IfTypeImplementsHostedServiceInterface()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<HostedServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await PublishMessageAsync(processor);

            instances.AssertInstanceCountIs(1);

            Assert.IsInstanceOfType(processor.ServiceProvider.GetRequiredService<IHostedService>(), typeof(HostedServiceBus));
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfServiceBusHasAlreadyBeenAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<HostedServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<HostedServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await PublishMessageAsync(processor);

            instances.AssertInstanceCountIs(1);

            Assert.IsInstanceOfType(processor.ServiceProvider.GetRequiredService<IEnumerable<IHostedService>>().Single(), typeof(HostedServiceBus));
        }

        [TestMethod]
        public async Task AddMicroServiceBus_BuildsExpectedServiceBus_IfMultipleServiceBusesAreAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<ScopedServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<SingletonServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.CreateScope())
            {
                await PublishMessageAsync(processor);
                await PublishMessageAsync(processor);
            }
            instances.AssertInstanceCountIs(4);

            using (processor.CreateScope())
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



        #endregion
    }
}

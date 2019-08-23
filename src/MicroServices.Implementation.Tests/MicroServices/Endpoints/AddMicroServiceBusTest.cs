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
            public abstract Task PublishAsync(IEnumerable<IMessage> messages);

            public abstract Task PublishAsync(IMessage message);
        }

        private sealed class GenericServiceBus<T> : IMicroServiceBus
        {
            public Task PublishAsync(IEnumerable<IMessage> messages) =>
                Task.CompletedTask;

            public Task PublishAsync(IMessage message) =>
                Task.CompletedTask;
        }

        private abstract class MicroServiceBus : MicroServiceBus<string>
        {
            private readonly IInstanceCollector _instances;

            protected MicroServiceBus(IInstanceCollector instances)
            {
                _instances = instances;
            }

            protected override Task ConnectAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            protected override Task DisconnectAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            protected override string Serialize(IMessage message) =>
                message.Instance.ToString();

            protected override Task PublishAsync(IEnumerable<string> messages)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }

            protected override Task PublishAsync(string message)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }
        }

        [MicroProcessorComponent]
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
        
        private sealed class SingletonServiceBus : MicroServiceBus
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

            await serviceBus.PublishAsync(new Event<object>(new object()));

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsAbstract()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<AbstractServiceBus>();

            var provider = BuildServiceProvider();
            var serviceBus = provider.GetRequiredService<IMicroServiceBus>();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            await serviceBus.PublishAsync(CreateMessage());

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsGeneric()
        {
            ProcessorBuilder.Components.AddMicroServiceBus(typeof(GenericServiceBus<>));

            var provider = BuildServiceProvider();
            var serviceBus = provider.GetRequiredService<IMicroServiceBus>();
            var instances = provider.GetRequiredService<IInstanceCollector>();

            await serviceBus.PublishAsync(CreateMessage());

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
        public async Task AddMicroServiceBus_AddsServiceBusAsHostedService_IfTypeImplementsHostedServiceInterface()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await PublishMessageAsync(processor);

            instances.AssertInstanceCountIs(1);

            Assert.IsInstanceOfType(processor.ServiceProvider.GetRequiredService<IHostedService>(), typeof(TransientServiceBus));
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfServiceBusHasAlreadyBeenAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();
            ProcessorBuilder.Components.AddMicroServiceBus<TransientServiceBus>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await PublishMessageAsync(processor);

            instances.AssertInstanceCountIs(1);

            Assert.IsInstanceOfType(processor.ServiceProvider.GetRequiredService<IEnumerable<IHostedService>>().Single(), typeof(TransientServiceBus));
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
            processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(CreateMessage());

        private static IMessage CreateMessage() =>
            CreateMessage(new object());

        private static IMessage CreateMessage<TMessage>(TMessage message) =>
            new Event<TMessage>(message);

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

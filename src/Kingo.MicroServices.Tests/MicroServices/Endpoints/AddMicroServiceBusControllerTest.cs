using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class AddMicroServiceBusControllerTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MicroProcessorController Types ======]

        private abstract class AbstractController : MicroServiceBusController
        {
            private readonly IMicroProcessor _processor;
            private readonly IInstanceCollector _instances;

            protected AbstractController(IMicroProcessor processor, IInstanceCollector instances)
            {
                _processor = processor;
                _instances = instances;
            }

            protected override IMicroProcessor Processor =>
                _processor;

            public override Task StartAsync(CancellationToken cancellationToken)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }                

            protected override HostedEndpoint CreateHostedEndpoint(HandleAsyncMethodEndpoint methodEndpoint) =>
                new HostedEndpointStub();
        }

        private sealed class GenericController<T> : AbstractController
        {
            public GenericController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }

        private sealed class HostedEndpointStub : HostedEndpoint
        {
            protected override Task ConnectAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            protected override Task DisconnectAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;
        }

        [MicroProcessorComponent(ServiceLifetime.Transient)]
        private sealed class TransientController : AbstractController
        {
            public TransientController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedController : AbstractController
        {
            public ScopedController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }
        
        private sealed class SingletonController : AbstractController
        {
            public SingletonController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }

        #endregion

        #region [====== AddMicroServiceBusController ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMicroServiceBusController_Throws_IfTypeIsNull()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(null);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsNoMicroServiceBusController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(typeof(object));

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsAbstract()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<AbstractController>();

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_DoesNothing_IfTypeIsGeneric()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(typeof(GenericController<>));

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsTransientServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(2);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsScopedServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_AddsServiceBusWithExpectedLifetime_IfTypeIsSingletonServiceBus()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(1);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor);
                await StartAllControllers(processor);
            }
            instances.AssertInstanceCountIs(1);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_DoesNothing_IfControllerHasAlreadyBeenAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor);

            instances.AssertInstanceCountIs(1);            
        }

        [TestMethod]
        public async Task AddMicroServiceBus_BuildsExpectedServiceBus_IfMultipleServiceBusesAreAdded()
        {
            const int controllerCount = 3;

            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();                        

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, controllerCount);
                await StartAllControllers(processor, controllerCount);
            }
            instances.AssertInstanceCountIs(4);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, controllerCount);
                await StartAllControllers(processor, controllerCount);
            }
            instances.AssertInstanceCountIs(7);
        }

        private static async Task StartAllControllers(IMicroProcessor processor, int expectedServiceCount = 1)
        {
            var serviceCount = 0;

            foreach (var service in ResolveServices(processor))
            {
                await service.StartAsync(CancellationToken.None);
                serviceCount++;
            }
            Assert.AreEqual(expectedServiceCount, serviceCount);
        }        

        private static IEnumerable<IHostedService> ResolveServices(IMicroProcessor processor) =>
            processor.ServiceProvider.GetRequiredService<IEnumerable<IHostedService>>();

        #endregion

        #region [====== AddMicroServiceBusControllers ======]

        [TestMethod]
        public async Task AddMicroServiceBusControllers_DoesNothing_IfNoTypesWereAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBusControllers();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 0);

            instances.AssertInstanceCountIs(0);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsExpectedControllers_IfMultipleControllerWereAreAddedAsType()
        {
            const int controllerCount = 3;

            ProcessorBuilder.Components.AddToSearchSet<TransientController>();
            ProcessorBuilder.Components.AddToSearchSet<ScopedController>();
            ProcessorBuilder.Components.AddToSearchSet<SingletonController>();
            ProcessorBuilder.Components.AddMicroServiceBusControllers();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, controllerCount);
                await StartAllControllers(processor, controllerCount);
            }
            instances.AssertInstanceCountIs(4);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, controllerCount);
                await StartAllControllers(processor, controllerCount);
            }
            instances.AssertInstanceCountIs(7);
        }

        #endregion
    }
}

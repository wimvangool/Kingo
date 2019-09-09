using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddMicroServiceBusControllerTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MicroServiceBusController Types ======]

        private abstract class AbstractController : MicroServiceBusController
        {
            private readonly IInstanceCollector _instances;

            protected AbstractController(IMicroProcessor processor, IMicroServiceBus bus, IInstanceCollector instances) :
                base(processor, bus)
            {
                _instances = instances;
            }

            public override Task StartAsync(CancellationToken cancellationToken)
            {
                _instances.Add(this);
                return Task.CompletedTask;
            }

            public override Task PublishAsync(IEnumerable<IMessageToDispatch> events)
            {
                _instances.Add(new object());
                return Task.CompletedTask;
            }

            protected override Task<IMicroServiceBusClient> CreateClientAsync(IMicroServiceBus bus) =>
                throw new NotSupportedException();
        }

        private sealed class GenericController<T> : AbstractController
        {
            public GenericController(IMicroProcessor processor, IMicroServiceBus bus, IInstanceCollector instances) :
                base(processor, bus, instances) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Transient)]
        private sealed class TransientController : AbstractController
        {
            public TransientController(IMicroProcessor processor, IMicroServiceBus bus, IInstanceCollector instances) :
                base(processor, bus, instances) { }
        }

        [MicroProcessorComponent(ServiceLifetime.Scoped)]
        private sealed class ScopedController : AbstractController
        {
            public ScopedController(IMicroProcessor processor, IMicroServiceBus bus, IInstanceCollector instances) :
                base(processor, bus, instances) { }
        }
        
        private sealed class SingletonController : AbstractController
        {
            public SingletonController(IMicroProcessor processor, IMicroServiceBus bus, IInstanceCollector instances) :
                base(processor, bus, instances) { }
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
        public async Task AddMicroServiceBusController_DoesNothing_IfTypeIsNoMicroServiceBusController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(typeof(object));

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_DoesNothing_IfTypeIsAbstract()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<AbstractController>();

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_DoesNothing_IfTypeIsGeneric()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(typeof(GenericController<>));

            var processor = CreateProcessor();

            await StartAllControllers(processor, 0);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsServiceBusWithExpectedLifetime_IfTypeIsTransientServiceBus()
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
        public async Task AddMicroServiceBusController_AddsServiceBusWithExpectedLifetime_IfTypeIsScopedServiceBus()
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
        public async Task AddMicroServiceBusController_AddsServiceBusWithExpectedLifetime_IfTypeIsSingletonServiceBus()
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task AddMicroServiceBus_DoesNotRegisterControllerAsMicroServiceBus_IfIsMainControllerIsFalse()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();

            var processor = CreateProcessor();
            var bus = processor.ServiceProvider.GetRequiredService<IMicroServiceBus>();

            try
            {
                await bus.PublishAsync(new object());
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual("Cannot publish specified event(s) because no instance or type implementing the 'IMicroServiceBus'-interface has been registered.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public async Task AddMicroServiceBus_RegistersControllerAsMicroServiceBus_IfIsMainControllerIsTrueForOneController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 3);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public async Task AddMicroServiceBus_RegistersControllersAsMicroServiceBus_IfIsMainControllerIsTrueForMultipleControllers()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 3);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(5);
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
    }
}

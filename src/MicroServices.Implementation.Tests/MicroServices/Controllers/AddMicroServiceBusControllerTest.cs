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

            protected AbstractController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor)
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

            protected override Task<IMicroServiceBusClient> CreateClientAsync() =>
                throw new NotSupportedException();
        }

        private sealed class GenericController<T> : AbstractController
        {
            public GenericController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }

        [MicroProcessorComponent]
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

        private sealed class SomeOtherController : AbstractController
        {
            public SomeOtherController(IMicroProcessor processor, IInstanceCollector instances) :
                base(processor, instances) { }
        }

        #endregion

        #region [====== AddMicroServiceBusController (Types) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMicroServiceBusController_Throws_IfTypeIsNull()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController(null);
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsNoMicroServiceBusController()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddMicroServiceBusController(typeof(object)));
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsAbstract()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddMicroServiceBusController<AbstractController>());
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsGeneric()
        {
            Assert.IsFalse(ProcessorBuilder.Components.AddMicroServiceBusController(typeof(GenericController<>)));
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsTrue_IfTypeConcreteController()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>());
        }

        #endregion

        #region [====== AddMicroServiceBusController (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfControllerIsTransient()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddMicroServiceBusController<TransientController>());

            BuildServiceProvider();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfControllerIsScoped()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddMicroServiceBusController<ScopedController>());

            BuildServiceProvider();
        }

        [TestMethod]
        public async Task BuildServiceProvider_ReturnsExpectedProvider_IfLifetimeOfControllerIsSingleton()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>());

            var processor = CreateProcessor();

            await StartAllControllers(processor, 1);
        }

        #endregion

        #region [====== AddMicroServiceBusController (Multiple Controllers & Main Controllers) ======]

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
        public async Task AddMicroServiceBusController_BuildsExpectedServiceBus_IfMultipleDifferentControllersAreAdded()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<SomeOtherController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();                        

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, 2);
                await StartAllControllers(processor, 2);
            }
            instances.AssertInstanceCountIs(2);

            using (processor.ServiceProvider.CreateScope())
            {
                await StartAllControllers(processor, 2);
                await StartAllControllers(processor, 2);
            }
            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfIsMainControllerIsTrue()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SomeOtherController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 2);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(3);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllersAsMicroServiceBus_IfIsMainControllerIsTrueForMultipleControllers()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SomeOtherController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 2);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfSameControllerIsAddedTwice_And_FirstIsAddedAsMainController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfSameControllerIsAddedTwice_And_SecondIsAddedAsMainController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>();
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfSameControllerIsAddedTwice_And_BothAreAddedAsMainController()
        {
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);
            ProcessorBuilder.Components.AddMicroServiceBusController<SingletonController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishAsync(new object());

            instances.AssertInstanceCountIs(2);
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

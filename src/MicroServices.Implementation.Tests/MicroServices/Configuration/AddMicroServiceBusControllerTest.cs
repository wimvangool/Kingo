using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.Controllers;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
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

            public override Task PublishEventsAsync(IEnumerable<IMessageToDispatch> events)
            {
                _instances.Add(new object());
                return Task.CompletedTask;
            }

            protected override Task<IMicroServiceBusClient> CreateClientAsync() =>
                throw new NotSupportedException();

            protected override TypeSet DefineServiceContract(TypeSet serviceContract) =>
                serviceContract;
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
            ProcessorBuilder.MicroServiceBusControllers.Add(null as Type);
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsNoMicroServiceBusController()
        {
            Assert.IsFalse(ProcessorBuilder.MicroServiceBusControllers.Add(typeof(object)));
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsAbstract()
        {
            Assert.IsFalse(ProcessorBuilder.MicroServiceBusControllers.Add<AbstractController>());
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsFalse_IfTypeIsGeneric()
        {
            Assert.IsFalse(ProcessorBuilder.MicroServiceBusControllers.Add(typeof(GenericController<>)));
        }

        [TestMethod]
        public void AddMicroServiceBusController_ReturnsTrue_IfTypeConcreteController()
        {
            Assert.IsTrue(ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>());
        }

        #endregion

        #region [====== AddMicroServiceBusController (Lifetime) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfControllerIsTransient()
        {
            Assert.IsTrue(ProcessorBuilder.MicroServiceBusControllers.Add<TransientController>());

            BuildServiceProvider();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BuildServiceProvider_Throws_IfLifetimeOfControllerIsScoped()
        {
            Assert.IsTrue(ProcessorBuilder.MicroServiceBusControllers.Add<ScopedController>());

            BuildServiceProvider();
        }

        [TestMethod]
        public async Task BuildServiceProvider_ReturnsExpectedProvider_IfLifetimeOfControllerIsSingleton()
        {
            Assert.IsTrue(ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>());

            var processor = CreateProcessor();

            await StartAllControllers(processor, 1);
        }

        #endregion

        #region [====== AddMicroServiceBusController (Multiple Controllers & Main Controllers) ======]

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsServiceBusWithExpectedLifetime_IfTypeIsSingletonServiceBus()
        {
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>();

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
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>();
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>();
            ProcessorBuilder.MicroServiceBusControllers.Add<SomeOtherController>();

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
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);
            ProcessorBuilder.MicroServiceBusControllers.Add<SomeOtherController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 2);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishEventsAsync(new object());

            instances.AssertInstanceCountIs(3);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllersAsMicroServiceBus_IfIsMainControllerIsTrueForMultipleControllers()
        {
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);
            ProcessorBuilder.MicroServiceBusControllers.Add<SomeOtherController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 2);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishEventsAsync(new object());

            instances.AssertInstanceCountIs(4);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfSameControllerIsAddedTwice_And_FirstIsAddedAsMainController()
        {
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>();

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishEventsAsync(new object());

            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsControllerAsMicroServiceBus_IfSameControllerIsAddedTwice_And_SecondIsAddedAsMainController()
        {
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>();
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishEventsAsync(new object());

            instances.AssertInstanceCountIs(2);
        }

        [TestMethod]
        public async Task AddMicroServiceBusController_AddsOnlySecondsController_IfSameControllerIsAddedTwice_And_BothAreAddedAsMainController()
        {
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);
            ProcessorBuilder.MicroServiceBusControllers.Add<SingletonController>(true);

            var processor = CreateProcessor();
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await StartAllControllers(processor, 1);
            await processor.ServiceProvider.GetRequiredService<IMicroServiceBus>().PublishEventsAsync(new object());

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

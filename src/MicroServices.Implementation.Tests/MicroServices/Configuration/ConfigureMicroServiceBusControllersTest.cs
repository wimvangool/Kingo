using System;
using System.Collections.Generic;
using Kingo.MicroServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class ConfigureMicroServiceBusControllersTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Stubs ======]

        private sealed class MicroServiceBusControllerStub : MicroServiceBusController
        {
            private readonly MicroServiceBusControllerOptions _options;

            public MicroServiceBusControllerStub(IMicroProcessor processor) : base(processor)
            {
                _options = new MicroServiceBusControllerOptions();
            }

            protected override MicroServiceBusControllerOptions Options =>
                _options;

            protected override MicroServiceBusClient CreateClient(IEnumerable<IMicroServiceBusEndpoint> endpoints) =>
                throw new NotSupportedException();
        }

        #endregion

        private readonly Lazy<IServiceProvider> _serviceProvider;

        public ConfigureMicroServiceBusControllersTest()
        {
            _serviceProvider = new Lazy<IServiceProvider>(CreateServiceProvider);
        }

        private IServiceProvider ServiceProvider =>
            _serviceProvider.Value;

        private IServiceProvider CreateServiceProvider() =>
            CreateProcessor().ServiceProvider;

        [TestMethod]
        public void Add_ReturnsFalse_IfTypeIsNoMicroServiceBusController()
        {
            Processor.ConfigureMicroServiceBusControllers(controllers =>
            {
                Assert.IsFalse(controllers.Add<object>());
            });
        }

        [TestMethod]
        public void Add_ReturnsTrue_IfTypeIsMicroServiceBusController()
        {
            Processor.ConfigureMicroServiceBusControllers(controllers =>
            {
                Assert.IsTrue(controllers.Add<MicroServiceBusControllerStub>());
            });

            var controllerA = ResolveMicroServiceBus<IMicroServiceBus>();
            var controllerB = ResolveMicroServiceBus<IHostedService>();

            Assert.IsNotNull(controllerA);
            Assert.IsNotNull(controllerB);
            Assert.AreSame(controllerA, controllerB);
        }

        private MicroServiceBusControllerStub ResolveMicroServiceBus<TController>() =>
            ServiceProvider.GetService<TController>() as MicroServiceBusControllerStub;
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class MicroProcessorConfigurationTest
    {
        private readonly MicroProcessorConfiguration _configuration;

        public MicroProcessorConfigurationTest()
        {
            _configuration = new MicroProcessorConfiguration();
        }

        #region [====== SetupMicroProcessor ======]

        [TestMethod]
        public void SetupMicroProcessor_ReturnsServiceConfigurator_IfInNotConfiguredState()
        {
            Assert.IsNotNull(_configuration.Setup());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetupMicroProcessor_Throws_IfInConfiguringState()
        {
            _configuration.Setup();
            _configuration.Setup();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetupMicroProcessor_Throws_IfInConfiguredState()
        {
            _configuration.Setup();
            _configuration.ServiceProvider();
            _configuration.Setup();
        }

        #endregion

        #region [====== Configure ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Configure_Throws_IfInNotConfiguredState()
        {
            _configuration.Configure(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Configure_Throws_IfInConfiguringState_And_ServiceConfiguratorIsNull()
        {
            _configuration.Setup().Configure(null);
        }

        [TestMethod]
        public void Configure_StoresTheServiceConfigurator_IfInConfiguringState_And_ServiceConfiguratorIsNotNull()
        {
            _configuration.Setup().Configure(services => { });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Configure_Throws_IfInConfiguringState_And_ServicesHaveAlreadyBeenConfigured()
        {
            _configuration.Setup().Configure(services => { });            
            _configuration.Configure(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Configure_Throws_IfInConfiguredState()
        {
            _configuration.Setup().Configure(services => { });
            _configuration.ServiceProvider();
            _configuration.Configure(null);
        }

        #endregion

        #region [====== ServiceProvider ======]

        [TestMethod]        
        public void ServiceProvider_ReturnsServiceProvider_IfInNotConfiguredState()
        {
            Assert.IsNotNull(_configuration.ServiceProvider());
        }

        [TestMethod]
        public void ServiceProvider_ReturnsServiceProvider_IfInConfiguringState()
        {
            _configuration.Setup();

            Assert.IsNotNull(_configuration.ServiceProvider());            
        }

        [TestMethod]
        public void ServiceProvider_ReturnsServiceProvider_IfInConfiguredState()
        {
            _configuration.Setup();

            Assert.IsNotNull(_configuration.ServiceProvider());
            Assert.IsNotNull(_configuration.ServiceProvider());
        }

        #endregion

        #region [====== ResolveProcessor ======]               

        [TestMethod]        
        public void ResolveProcessor_ReturnsDefaultProcessor_IfInNotConfiguredState()
        {
            var processor = _configuration.ResolveProcessor();

            Assert.IsInstanceOfType(processor, typeof(MicroProcessor));
        }

        [TestMethod]        
        public void ResolveProcessor_ReturnsExpectedProcessor_IfOnlyBasicProcessorHasBeenConfigured()
        {
            _configuration.Setup();

            var processor = _configuration.ResolveProcessor();

            Assert.IsInstanceOfType(processor, typeof(MicroProcessor));
        }

        [TestMethod]
        public async Task ResolveProcessor_ReturnsExpectedProcessor_IfSomeProcessorPropertiesHaveBeenConfigured()
        {
            var bus = new MicroServiceBusStub();
            var @event = new object();

            _configuration.Setup(processor =>
            {
                processor.ServiceBus.Add(bus);
            });

            Assert.AreEqual(1, await _configuration.ResolveProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(@event);
            }));

            bus.AssertEventCountIs(1);
            bus.AssertAreSame(0, @event);
        }

        [TestMethod]
        public async Task ResolveProcessor_ReturnsExpectedProcessor_IfSomeServicesHaveBeenConfigured()
        {
            var bus = new MicroServiceBusStub();
            var @event = new object();

            _configuration.Setup<CustomProcessor>(processor =>
            {
                processor.ServiceBus.Add(bus);

            }).Configure(services =>
            {
                services.AddTransient<IEventStreamProcessor, EventStreamDuplicator>();
            });

            Assert.AreEqual(1, await _configuration.ResolveProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(@event);
            }));

            bus.AssertEventCountIs(2);
            bus.AssertAreSame(0, @event);
            bus.AssertAreSame(1, @event);
        }

        #endregion
    }
}

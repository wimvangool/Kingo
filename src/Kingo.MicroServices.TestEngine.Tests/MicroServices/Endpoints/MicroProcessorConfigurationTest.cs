using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
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
            var @event = new object();

            _configuration.Setup();

            var result = await _configuration.ResolveProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.EventBus.Publish(@event);
            }, new object());

            Assert.AreEqual(1, result.MessageHandlerCount);
            Assert.AreEqual(1, result.Events.Count);
            Assert.AreSame(@event, result.Events[0].Instance);            
        }        

        #endregion
    }
}

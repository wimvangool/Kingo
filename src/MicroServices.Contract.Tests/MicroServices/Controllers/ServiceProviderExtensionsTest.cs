using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class ServiceProviderExtensionsTest
    {
        #region [====== MicroServiceBus ======]

        private sealed class MicroServiceBus : IMicroServiceBus
        {
            private readonly List<object> _events;

            public MicroServiceBus()
            {
                _events = new List<object>();
            }

            public void AssertHasEvent(object @event) =>
                Assert.IsTrue(_events.Contains(@event));

            public async Task PublishAsync(IEnumerable<object> events)
            {
                foreach (var @event in events)
                {
                    await PublishAsync(@event);
                }
            }

            public Task PublishAsync(object @event)
            {
                _events.Add(@event);
                return Task.CompletedTask;
            }
        }

        #endregion

        private readonly IServiceCollection _services;

        public ServiceProviderExtensionsTest()
        {
            _services = new ServiceCollection();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMicroServiceBus_Throws_IfProviderIsNull()
        {
            ServiceProviderExtensions.GetMicroServiceBus(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetMicroServiceBus_ReturnsMicroServiceBusStub_IfNoServiceBusWasRegistered()
        {
            await CreateProvider().GetMicroServiceBus().PublishAsync(new object());
        }

        [TestMethod]
        public async Task GetMicroServiceBus_ReturnsRegisteredServiceBus_IfOneServiceBusWasRegistered()
        {
            var @event = new object();
            var bus = new MicroServiceBus();

            _services.AddSingleton<IMicroServiceBus>(bus);

            await CreateProvider().GetMicroServiceBus().PublishAsync(@event);

            bus.AssertHasEvent(@event);
        }

        [TestMethod]
        public async Task GetMicroServiceBus_ReturnsCompositeServiceBus_IfMultipleServiceBusesWereRegistered()
        {
            var @event = new object();
            var busA = new MicroServiceBus();
            var busB = new MicroServiceBus();

            _services.AddSingleton<IMicroServiceBus>(busA);
            _services.AddSingleton<IMicroServiceBus>(busB);

            await CreateProvider().GetMicroServiceBus().PublishAsync(@event);

            busA.AssertHasEvent(@event);
            busB.AssertHasEvent(@event);
        }

        private IServiceProvider CreateProvider() =>
            _services.BuildServiceProvider();
    }
}

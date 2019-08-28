using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddMicroServiceBusTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== MicroServiceBusTypes ======]

        private sealed class MicroServiceBusOne : IMicroServiceBus
        {
            public MicroServiceBusOne(IMicroServiceBus bus)
            {
                bus.PublishAsync(new object()).GetAwaiter().GetResult();
            }

            public Task PublishAsync(IEnumerable<object> events) =>
                Task.CompletedTask;

            public Task PublishAsync(object @event) =>
                Task.CompletedTask;
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetServiceBus_Throws_IfBusAttemptsToPublishEventInsideConstructor()
        {
            Assert.IsTrue(ProcessorBuilder.Components.AddMicroServiceBus<MicroServiceBusOne>());

            var processor = CreateProcessor();

            try
            {
                processor.ServiceProvider.GetService<MicroServiceBusOne>();
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual("Cannot publish the specified event(s) inside the constructor of a type that is registered as a 'IMicroServiceBus'-type, because this causes a circular reference.", exception.Message);
                Assert.IsNotNull(exception.InnerException);
                throw;
            }
        }
    }
}

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorComponentTest
    {
        #region [====== Component Types ======]

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IMessageHandler<>))]
        private sealed class Component1 { }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IDisposable))]
        private sealed class Component2 { }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IDisposable))]
        private sealed class Component3 : IDisposable
        {
            public void Dispose() { }
        }

        [MicroProcessorComponent(ServiceLifetime.Transient, typeof(IDisposable), typeof(IDisposable))]
        private sealed class Component4 : IDisposable
        {
            public void Dispose() { }
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromInstance_Throws_IfTypeIsValueType()
        {
            MicroProcessorComponent.FromInstance(0);
        }

        [TestMethod]
        public void ServiceTypes_ReturnsEmptyCollection_IfServiceTypesAreNotSpecified()
        {
            var component = MicroProcessorComponent.FromInstance(new object());

            Assert.AreEqual(0, component.ServiceTypes.Count());
        }

        [TestMethod]
        public void ServiceTypes_ReturnsEmptyCollection_IfServiceTypeIsGenericType()
        {
            var component = MicroProcessorComponent.FromInstance(new Component1());

            Assert.AreEqual(0, component.ServiceTypes.Count());
        }

        [TestMethod]
        public void ServiceTypes_ReturnsEmptyCollection_IfSpecifiedServiceTypeIsNotImplementedByComponent()
        {
            var component = MicroProcessorComponent.FromInstance(new Component2());

            Assert.AreEqual(0, component.ServiceTypes.Count());
        }        

        [TestMethod]
        public void ServiceTypes_ReturnsExpectedServiceType_IfServiceTypeIsImplementedByComponent()
        {
            var component = MicroProcessorComponent.FromInstance(new Component3());

            Assert.AreSame(typeof(IDisposable), component.ServiceTypes.Single());
        }

        [TestMethod]
        public void ServiceTypes_ReturnsExpectedServiceType_IfServiceTypeIsSpecifiedMoreThanOnce()
        {
            var component = MicroProcessorComponent.FromInstance(new Component4());

            Assert.AreSame(typeof(IDisposable), component.ServiceTypes.Single());
        }
    }
}

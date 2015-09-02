using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
{
    [TestClass]
    public sealed class MessageHandlerInstanceTest
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        private sealed class SingleValueAttribute : Attribute
        {
            internal readonly int Value;

            internal SingleValueAttribute(int value)
            {
                Value = value;
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
        private sealed class MultiValueAttribute : Attribute
        {
            internal readonly int Value;

            internal MultiValueAttribute(int value)
            {
                Value = value;
            }
        }

        [SingleValue(1)]
        [MultiValue(1)]
        [MultiValue(2)]
        private sealed class MessageHandlerWithAttributes : IMessageHandler<EmptyMessage>, IMessageHandler<object>
        {
            [SingleValue(2)]
            [MultiValue(3)]
            [MultiValue(4)]
            public Task HandleAsync(EmptyMessage message)
            {
                return AsyncMethod.Void;
            }

            [SingleValue(3)]
            [MultiValue(5)]
            [MultiValue(6)]
            public Task HandleAsync(object message)
            {
                return AsyncMethod.Void;
            }
        }

        private MessageHandlerWithAttributes _handler;

        [TestInitialize]
        public void Setup()
        {
            _handler = new MessageHandlerWithAttributes();
        }

        #region [====== GetClassAttributes ======]

        [TestMethod]
        public void GetClassAttributes_ReturnsAllClassAttributes_IfAllowMultipleIsFalse()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            var attributes = instance.GetClassAttributesOfType<SingleValueAttribute>().ToArray();

            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetClassAttributes_ReturnsAllClassAttributes_IfAllowMultipleIsTrue()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            var attributes = instance.GetClassAttributesOfType<MultiValueAttribute>().ToArray();

            Assert.AreEqual(2, attributes.Length);
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 1));
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 2));
        }

        #endregion

        #region [====== GetMethodAttributes ======]

        [TestMethod]
        public void GetMethodAttributes_ReturnsAllMethodAttributes_IfAllowMultipleIsFalse_And_MessageIsOfExactDerivedType()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            var attributes = instance.GetMethodAttributesOfType<SingleValueAttribute>().ToArray();

            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(2, attributes[0].Value);
        }

        [TestMethod]
        public void GetMethodAttributes_ReturnsAllMethodAttributes_IfAllowMultipleIsFalse_And_MessageIsOfExactBaseType()
        {
            var instance = new MessageHandlerInstance<object>(_handler);
            var attributes = instance.GetMethodAttributesOfType<SingleValueAttribute>().ToArray();

            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(3, attributes[0].Value);
        }

        [TestMethod]
        public void GetMethodAttributes_ReturnsAllMethodAttributes_IfAllowMultipleIsTrue_And_MessageIsOfExactDerivedType()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            var attributes = instance.GetMethodAttributesOfType<MultiValueAttribute>().ToArray();

            Assert.AreEqual(2, attributes.Length);
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 3));
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 4));
        }

        [TestMethod]
        public void GetMethodAttributes_ReturnsAllMethodAttributes_IfAllowMultipleIsTrue_And_MessageIsOfExactBaseType()
        {
            var instance = new MessageHandlerInstance<object>(_handler);
            var attributes = instance.GetMethodAttributesOfType<MultiValueAttribute>().ToArray();

            Assert.AreEqual(2, attributes.Length);
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 5));
            Assert.IsTrue(attributes.Any(attribute => attribute.Value == 6));
        }

        #endregion

        #region [====== TryGetClassAttribute ======]

        [TestMethod]
        public void TryGetClassAttribute_ReturnsFalse_IfAttributeWasNotFound()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            string attribute;

            Assert.IsFalse(instance.TryGetClassAttributeOfType(out attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetClassAttribute_ReturnsTrue_IfAttributeWasFound()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            SingleValueAttribute attribute;

            Assert.IsTrue(instance.TryGetClassAttributeOfType(out attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(AmbiguousMatchException))]
        public void TryGetClassAttribute_Throws_IfMultipleAttributesMatchWithType()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            MultiValueAttribute attribute;

            instance.TryGetClassAttributeOfType(out attribute);
        }

        #endregion

        #region [====== TryGetMethodAttribute ======]

        [TestMethod]
        public void TryGetMethodAttribute_ReturnsFalse_IfAttributeWasNotFound()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            string attribute;

            Assert.IsFalse(instance.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetMethodAttribute_ReturnsTrue_IfAttributeWasFound()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            SingleValueAttribute attribute;

            Assert.IsTrue(instance.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(2, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(AmbiguousMatchException))]
        public void TryGetMethodAttribute_Throws_IfMultipleAttributesMatchWithType()
        {
            var instance = new MessageHandlerInstance<EmptyMessage>(_handler);
            MultiValueAttribute attribute;

            instance.TryGetMethodAttributeOfType(out attribute);
        }

        #endregion
    }
}

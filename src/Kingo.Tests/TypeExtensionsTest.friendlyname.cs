using System;
using System.Collections.Generic;
using Kingo.MicroServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    public sealed partial class TypeExtensionsTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FriendlyName_Throws_IfTypeIsNull()
        {
            TypeExtensions.FriendlyName(null);
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsNotGeneric_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("Object", typeof(object).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsNotGeneric_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Object", typeof(object).FriendlyName(true));
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericTypeDefinition_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("IMessageHandler<TMessage>", typeof(IMessageHandler<>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericTypeDefinition_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("Kingo.MicroServices.IMessageHandler<TMessage>", typeof(IMessageHandler<>).FriendlyName(true));
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericTypeDefinition_And_TypeHasManyGenericTypeParameters_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("KeyValuePair<TKey, TValue>", typeof(KeyValuePair<,>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericTypeDefinition_And_TypeHasManyGenericTypeParameters_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Collections.Generic.KeyValuePair<TKey, TValue>", typeof(KeyValuePair<,>).FriendlyName(true));
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("IMessageHandler<Int32>", typeof(IMessageHandler<int>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("Kingo.MicroServices.IMessageHandler<System.Int32>", typeof(IMessageHandler<int>).FriendlyName(true));
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericType_And_TypeHasManyGenericTypeParameters_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("KeyValuePair<Int32, String>", typeof(KeyValuePair<int, string>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericType_And_TypeHasManyGenericTypeParameters_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Collections.Generic.KeyValuePair<System.Int32, System.String>", typeof(KeyValuePair<int, string>).FriendlyName(true));
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsNestedGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsFalse()
        {
            Assert.AreEqual("IMessageHandler<KeyValuePair<Object, Int16>>", typeof(IMessageHandler<KeyValuePair<object, short>>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsNestedGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("Kingo.MicroServices.IMessageHandler<System.Collections.Generic.KeyValuePair<System.Object, System.Int16>>", typeof(IMessageHandler<KeyValuePair<object, short>>).FriendlyName(true));
        }
    }
}

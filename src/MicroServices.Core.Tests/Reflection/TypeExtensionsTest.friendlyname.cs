using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Reflection
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
            Assert.AreEqual("IList<T>", typeof(IList<>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericTypeDefinition_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Collections.Generic.IList<T>", typeof(IList<>).FriendlyName(true));
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
            Assert.AreEqual("IList<Int32>", typeof(IList<int>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Collections.Generic.IList<System.Int32>", typeof(IList<int>).FriendlyName(true));
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
            Assert.AreEqual("IList<KeyValuePair<Object, Int16>>", typeof(IList<KeyValuePair<object, short>>).FriendlyName());
        }

        [TestMethod]
        public void FriendlyName_ReturnsExpectedName_IfTypeIsNestedGenericType_And_TypeHasOneGenericTypeParameter_And_UseFullNamesIsTrue()
        {
            Assert.AreEqual("System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<System.Object, System.Int16>>", typeof(IList<KeyValuePair<object, short>>).FriendlyName(true));
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Reflection
{
    [TestClass]
    public sealed class TypeAttributeProviderTest
    {
        #region [====== Attributes ======]

        private interface IHasValue
        {
            int Value
            {
                get;
            }
        }

        [AttributeUsage(AttributeTargets.Class)]
        private sealed class AllowOneAttribute : Attribute, IHasValue
        {
            public AllowOneAttribute(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
            }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private sealed class AllowManyAttribute : Attribute, IHasValue
        {
            public AllowManyAttribute(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
            }
        }

        #endregion

        #region [====== Classes ======]

        private sealed class NoAttributeClass { }

        [AllowOne(1)]
        private sealed class OneAttributeClass { }

        [AllowOne(2)]
        [AllowMany(3)]
        [AllowMany(4)]
        private sealed class ManyAttributesClass { }

        #endregion

        #region [====== TryGetAttributeOfType ======]

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfClassDoesNotHaveAnyAttributesAtAll()
        {
            var provider = new TypeAttributeProvider(typeof(NoAttributeClass));            

            Assert.IsFalse(provider.TryGetAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfClassDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = new TypeAttributeProvider(typeof(OneAttributeClass));            

            Assert.IsFalse(provider.TryGetAttributeOfType(out AllowManyAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfClassHasExactlyOneAttributesOfTheSpecifiedConcreteType()
        {
            var provider = new TypeAttributeProvider(typeof(OneAttributeClass));            

            Assert.IsTrue(provider.TryGetAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfClassHasExactlyOneAttributesOfTheSpecifiedInterfaceType()
        {
            var provider = new TypeAttributeProvider(typeof(OneAttributeClass));            

            Assert.IsTrue(provider.TryGetAttributeOfType(out IHasValue attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetTypeAttributeOfType_Throws_IfClassHasManyAttributesOfTheSpecifiedType()
        {
            var provider = new TypeAttributeProvider(typeof(ManyAttributesClass));             

            provider.TryGetAttributeOfType(out AllowManyAttribute attribute);
        }

        #endregion

        #region [====== GetAttributesOfType ======]

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsEmptyCollection_IfClassDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = new TypeAttributeProvider(typeof(NoAttributeClass));
            var attributes = provider.GetAttributesOfType<AllowOneAttribute>();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfClassHasOneAttributeOfTheSpecifiedConcreteType()
        {
            var provider = new TypeAttributeProvider(typeof(OneAttributeClass));
            var attributes = provider.GetAttributesOfType<AllowOneAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfClassHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = new TypeAttributeProvider(typeof(OneAttributeClass));
            var attributes = provider.GetAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfClassHasManyAttributesOfTheSpecifiedConcreteType()
        {
            var provider = new TypeAttributeProvider(typeof(ManyAttributesClass));
            var attributes = provider.GetAttributesOfType<AllowManyAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(2, attributes.Length);
            Assert.AreEqual(3, attributes[0].Value);
            Assert.AreEqual(4, attributes[1].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfClassHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = new TypeAttributeProvider(typeof(ManyAttributesClass));
            var attributes = provider.GetAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(3, attributes.Length);
            Assert.AreEqual(2, attributes[0].Value);
            Assert.AreEqual(3, attributes[1].Value);
            Assert.AreEqual(4, attributes[2].Value);
        }

        #endregion
    }
}

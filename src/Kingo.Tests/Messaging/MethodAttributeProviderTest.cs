using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MethodAttributeProviderTest
    {
        #region [====== Attributes ======]

        private interface IHasValue
        {
            int Value
            {
                get;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
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

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
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

        #region [====== MessageHandlers ======]

        private sealed class NoAttributeHandler : IMessageHandler<object>
        {
            Task IMessageHandler<object>.HandleAsync(object message, IMicroProcessorContext context) =>
                NoValue;
        }

        private sealed class OneAttributeHandler : IMessageHandler<object>
        {
            [AllowOne(1)]
            public Task HandleAsync(object message, IMicroProcessorContext context) =>
                NoValue;
        }

        private sealed class ManyAttributesHandler : IMessageHandler<object>
        {
            [AllowOne(2)]
            [AllowMany(3)]
            [AllowMany(4)]
            public Task HandleAsync(object message, IMicroProcessorContext context) =>
                NoValue;
        }

        #endregion

        #region [====== Queries (1) ======]

        private sealed class NoAttributeQuery1 : IQuery<object>
        {
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                Value(new object());
        }

        private sealed class OneAttributeQuery1 : IQuery<object>
        {
            [AllowOne(1)]
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                Value(new object());
        }

        private sealed class ManyAttributesQuery1 : IQuery<object>
        {
            [AllowOne(2)]
            [AllowMany(3)]
            [AllowMany(4)]
            public Task<object> ExecuteAsync(IMicroProcessorContext context) =>
                Value(new object());
        }

        #endregion

        #region [====== Queries (2) ======]

        private sealed class NoAttributeQuery2 : IQuery<object, object>
        {
            public Task<object> ExecuteAsync(object message, IMicroProcessorContext context) =>
                Value(new object());
        }

        private sealed class OneAttributeQuery2 : IQuery<object, object>
        {
            [AllowOne(1)]
            public Task<object> ExecuteAsync(object message, IMicroProcessorContext context) =>
                Value(new object());
        }

        private sealed class ManyAttributesQuery2 : IQuery<object, object>
        {
            [AllowOne(2)]
            [AllowMany(3)]
            [AllowMany(4)]
            public Task<object> ExecuteAsync(object message, IMicroProcessorContext context) =>
                Value(new object());
        }

        #endregion

        #region [====== TryGetMethodAttributeOfType - MessageHandlers ======]

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsMessageHandler_And_MethodDoesNotHaveAnyAttributesAtAll()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new NoAttributeHandler());             

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsMessageHandler_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new OneAttributeHandler());             

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out AllowManyAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsMessageHandler_And_MethodHasExactlyOneAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new OneAttributeHandler());            

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsMessageHandler_And_MethodHasExactlyOneAttributesOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new OneAttributeHandler());            

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out IHasValue attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetTypeAttributeOfType_Throws_IfTypeIsMessageHandler_And_MethodHasManyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new ManyAttributesHandler());             

            provider.TryGetMethodAttributeOfType(out AllowManyAttribute attribute);
        }

        #endregion

        #region [====== TryGetMethodAttributeOfType - Queries (1) ======]

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsQuery1_And_MethodDoesNotHaveAnyAttributesAtAll()
        {
            var provider = MethodAttributeProvider.FromQuery(new NoAttributeQuery1());
            AllowOneAttribute attribute;

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsQuery1_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery1());
            AllowManyAttribute attribute;

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsQuery1_And_MethodHasExactlyOneAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery1());
            AllowOneAttribute attribute;

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsQuery1_And_MethodHasExactlyOneAttributesOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery1());
            IHasValue attribute;

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetTypeAttributeOfType_Throws_IfTypeIsQuery1_And_MethodHasManyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery1());
            AllowManyAttribute attribute;

            provider.TryGetMethodAttributeOfType(out attribute);
        }

        #endregion

        #region [====== TryGetMethodAttributeOfType - Queries (2) ======]

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsQuery2_And_MethodDoesNotHaveAnyAttributesAtAll()
        {
            var provider = MethodAttributeProvider.FromQuery(new NoAttributeQuery2());            

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsFalse_IfTypeIsQuery2_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery2());            

            Assert.IsFalse(provider.TryGetMethodAttributeOfType(out AllowManyAttribute attribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsQuery2_And_MethodHasExactlyOneAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery2());            

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out AllowOneAttribute attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        public void TryGetTypeAttributeOfType_ReturnsTrue_IfTypeIsQuery2_And_MethodHasExactlyOneAttributesOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery2());            

            Assert.IsTrue(provider.TryGetMethodAttributeOfType(out IHasValue attribute));
            Assert.IsNotNull(attribute);
            Assert.AreEqual(1, attribute.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetTypeAttributeOfType_Throws_IfTypeIsQuery2_And_MethodHasManyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery2());            

            provider.TryGetMethodAttributeOfType(out AllowManyAttribute attribute);
        }

        #endregion

        #region [====== GetMethodAttributesOfType - MessageHandlers ======]

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsEmptyCollection_IfTypeIsMessageHandler_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new NoAttributeHandler());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsMessageHandler_And_MethodHasOneAttributeOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new OneAttributeHandler());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsMessageHandler_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new OneAttributeHandler());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsMessageHandler_And_MethodHasManyAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new ManyAttributesHandler());
            var attributes = provider.GetMethodAttributesOfType<AllowManyAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(2, attributes.Length);
            Assert.AreEqual(3, attributes[0].Value);
            Assert.AreEqual(4, attributes[1].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsMessageHandler_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromMessageHandler(new ManyAttributesHandler());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(3, attributes.Length);
            Assert.AreEqual(2, attributes[0].Value);
            Assert.AreEqual(3, attributes[1].Value);
            Assert.AreEqual(4, attributes[2].Value);
        }

        #endregion

        #region [====== GetMethodAttributesOfType - Queries (1) ======]

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsEmptyCollection_IfTypeIsQuery1_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new NoAttributeQuery1());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsQuery1_And_MethodHasOneAttributeOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery1());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsQuery1_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery1());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsQuery1_And_MethodHasManyAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery1());
            var attributes = provider.GetMethodAttributesOfType<AllowManyAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(2, attributes.Length);
            Assert.AreEqual(3, attributes[0].Value);
            Assert.AreEqual(4, attributes[1].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsQuery1_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery1());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(3, attributes.Length);
            Assert.AreEqual(2, attributes[0].Value);
            Assert.AreEqual(3, attributes[1].Value);
            Assert.AreEqual(4, attributes[2].Value);
        }

        #endregion

        #region [====== GetMethodAttributesOfType - Queries (2) ======]

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsEmptyCollection_IfTypeIsQuery2_And_MethodDoesNotHaveAnyAttributesOfTheSpecifiedType()
        {
            var provider = MethodAttributeProvider.FromQuery(new NoAttributeQuery2());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(0, attributes.Count());
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsQuery2_And_MethodHasOneAttributeOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery2());
            var attributes = provider.GetMethodAttributesOfType<AllowOneAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsOneItem_IfTypeIsQuery2_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new OneAttributeQuery2());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(1, attributes[0].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsQuery2_And_MethodHasManyAttributesOfTheSpecifiedConcreteType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery2());
            var attributes = provider.GetMethodAttributesOfType<AllowManyAttribute>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(2, attributes.Length);
            Assert.AreEqual(3, attributes[0].Value);
            Assert.AreEqual(4, attributes[1].Value);
        }

        [TestMethod]
        public void GetTypeAttributesOfType_ReturnsManyItems_IfTypeIsQuery2_And_MethodHasOneAttributeOfTheSpecifiedInterfaceType()
        {
            var provider = MethodAttributeProvider.FromQuery(new ManyAttributesQuery2());
            var attributes = provider.GetMethodAttributesOfType<IHasValue>().ToArray();

            Assert.IsNotNull(attributes);
            Assert.AreEqual(3, attributes.Length);
            Assert.AreEqual(2, attributes[0].Value);
            Assert.AreEqual(3, attributes[1].Value);
            Assert.AreEqual(4, attributes[2].Value);
        }

        #endregion
    }
}

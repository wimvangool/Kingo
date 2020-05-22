using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class SerializerFactoryTest
    {
        private readonly SerializerTypeMap _serializerTypeMap;
        private readonly ServiceCollection _services;

        public SerializerFactoryTest()
        {
            _serializerTypeMap = new SerializerTypeMap();
            _services = new ServiceCollection();
        }

        #region [====== CreateSerializerFor ======]

        [TestMethod]
        public void CreateSerializerFor_ReturnsDefaultSerializer_IfNoSerializerWasAdded_And_AssociatedTypeIsObject()
        {
            var serializer = CreateSerializerFactory().CreateSerializerFor(typeof(object));

            Assert.IsInstanceOfType(serializer, typeof(JsonFormatSerializer));
        }

        [TestMethod]
        public void CreateSerializerFor_ReturnsDefaultSerializer_IfSerializerWasAdded_But_AssociatedTypeDoesNotMatchAddedType()
        {
            if (SerializerType.IsSerializerType(typeof(XmlFormatSerializer), out var serializerType))
            {
                _serializerTypeMap.Add(serializerType, typeof(int));
            }

            var serializer = CreateSerializerFactory().CreateSerializerFor(typeof(object));

            Assert.IsInstanceOfType(serializer, typeof(JsonFormatSerializer));
        }

        [TestMethod]
        public void CreateSerializerFor_ReturnsDefaultSerializer_IfSerializerWasAdded_And_AssociatedTypeMatchesAddedType_But_SerializerWasNotRegistered()
        {
            if (SerializerType.IsSerializerType(typeof(XmlFormatSerializer), out var serializerType))
            {
                _serializerTypeMap.Add(serializerType, typeof(int));
            }

            var serializer = CreateSerializerFactory().CreateSerializerFor(typeof(int));

            Assert.IsInstanceOfType(serializer, typeof(JsonFormatSerializer));
        }

        [TestMethod]
        public void CreateSerializerFor_ReturnsExpectedSerializer_IfSerializerWasAdded_And_AssociatedTypeMatchesAddedType_And_SerializerWasRegistered()
        {
            if (SerializerType.IsSerializerType(typeof(XmlFormatSerializer), out var serializerType))
            {
                _serializerTypeMap.Add(serializerType, typeof(int));
            }

            _services.AddTransient<XmlFormatSerializer>();

            var serializer = CreateSerializerFactory().CreateSerializerFor(typeof(int));

            Assert.IsInstanceOfType(serializer, typeof(XmlFormatSerializer));
        }

        [TestMethod]
        public void CreateSerializerFor_ReturnsExpectedSerializer_IfSerializerWasAdded_And_AssociatedTypeIsSubTypeOfAddedType_And_SerializerWasRegistered()
        {
            if (SerializerType.IsSerializerType(typeof(BinaryFormatSerializer), out var serializerType))
            {
                _serializerTypeMap.Add(serializerType, typeof(object));
            }

            _services.AddTransient<BinaryFormatSerializer>();

            var serializer = CreateSerializerFactory().CreateSerializerFor(typeof(int));

            Assert.IsInstanceOfType(serializer, typeof(BinaryFormatSerializer));
        }

        #endregion

        private ISerializerFactory CreateSerializerFactory() =>
            new SerializerFactory(_serializerTypeMap, _services.BuildServiceProvider());
    }
}

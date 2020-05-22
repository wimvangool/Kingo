using System;
using Kingo.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class ConfigureSerializersTest : MicroProcessorTest<MicroProcessor>
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfTypeIsNull()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(null);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Add_DoesNotAddMapping_IfTypeIsNoSerializerType()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(object)).For<object>();
            });
        }

        [TestMethod]
        public void Add_AddsTypeMapping_IfTypeIsSerializerType()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(JsonFormatSerializer)).For<object>();
            });

            var serializer = CreateSerializerFor(typeof(object));

            Assert.IsInstanceOfType(serializer, typeof(JsonFormatSerializer));
        }

        [TestMethod]
        public void Add_AddsAnotherTypeMapping_IfSerializerTypeIsAddedASecondTime()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(XmlFormatSerializer)).For<string>();
                serializers.Add(typeof(XmlFormatSerializer)).For<int>();
            });

            Assert.IsInstanceOfType(CreateSerializerFor(typeof(object)), typeof(JsonFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(string)), typeof(XmlFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(int)), typeof(XmlFormatSerializer));
        }

        [TestMethod]
        public void Add_AddAssociatesMultipleTypesWithSerializer_IfMultipleTypesAreSpecifiedInSingleForStatement()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(XmlFormatSerializer)).For(typeof(string), typeof(int));
            });

            Assert.IsInstanceOfType(CreateSerializerFor(typeof(object)), typeof(JsonFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(string)), typeof(XmlFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(int)), typeof(XmlFormatSerializer));
        }

        [TestMethod]
        public void Add_AddAssociatesMultipleTypesWithSerializer_IfMultipleTypesAreSpecifiedInSeparateForStatements()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(BinaryFormatSerializer)).For<string>().For<int>();
            });

            Assert.IsInstanceOfType(CreateSerializerFor(typeof(object)), typeof(JsonFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(string)), typeof(BinaryFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(int)), typeof(BinaryFormatSerializer));
        }

        [TestMethod]
        public void Add_OverwritesAssociationWithSerializer_IfSecondStatementsAssociatedTypeWithAnotherSerializer()
        {
            Processor.ConfigureSerializers(serializers =>
            {
                serializers.Add(typeof(BinaryFormatSerializer)).For<string>();
                serializers.Add(typeof(XmlFormatSerializer)).For<string>();
            });

            Assert.IsInstanceOfType(CreateSerializerFor(typeof(object)), typeof(JsonFormatSerializer));
            Assert.IsInstanceOfType(CreateSerializerFor(typeof(string)), typeof(XmlFormatSerializer));
        }

        private ISerializer CreateSerializerFor(Type associatedType = null) =>
            CreateProcessor().ServiceProvider.GetService<ISerializerFactory>().CreateSerializerFor(associatedType);
    }
}

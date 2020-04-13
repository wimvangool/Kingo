using System;
using System.Runtime.Serialization;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class SerializerExtensionsTest
    {
        private readonly BinaryFormatSerializer _serializer;

        public SerializerExtensionsTest()
        {
            _serializer = new BinaryFormatSerializer();
        }

        #region [====== Deserialize ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfSerializerIsNull()
        {
            SerializerExtensions.Deserialize<object>(null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfValueIsNull()
        {
            _serializer.Deserialize<object>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeDeserialized()
        {
            _serializer.Deserialize<object>(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeDeserializedToSpecifiedType()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = _serializer.Serialize(value);

            _serializer.Deserialize<SomeCustomObject>(serializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueCanBeDeserializedToSpecifiedType()
        {
            var value = new SomeCustomObject();
            var serializedValue = _serializer.Serialize(value);
            var deserializedValue = _serializer.Deserialize<SomeCustomObject>(serializedValue);

            Assert.AreNotSame(value, deserializedValue);
            Assert.AreEqual(value.Id, deserializedValue.Id);
        }

        #endregion

        #region [====== Copy ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Throws_IfSerializerIsNull()
        {
            SerializerExtensions.Copy(null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Copy_Throws_IfInstanceIsNull()
        {
            _serializer.Copy(default(object));
        }

        [TestMethod]
        public void Copy_ReturnsDeepCopy_IfInstanceIsObject()
        {
            var valueA = new object();
            var valueB = _serializer.Copy(valueA);

            Assert.AreNotSame(valueA, valueB);
            Assert.AreSame(valueA.GetType(), valueB.GetType());
        }

        [TestMethod]
        public void Copy_ReturnsDeepCopy_IfInstanceIsSomeCustomObject()
        {
            var valueA = new SomeCustomObject();
            var valueB = _serializer.Copy(valueA);

            Assert.AreNotSame(valueA, valueB);
            Assert.AreSame(valueA.GetType(), valueB.GetType());
            Assert.AreEqual(valueA.Id, valueB.Id);
        }

        #endregion
    }
}

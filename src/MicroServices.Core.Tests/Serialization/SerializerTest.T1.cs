using System;
using System.Runtime.Serialization;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public abstract class SerializerTest<TSerializer> where TSerializer : class, ISerializer
    {
        protected abstract TSerializer Serializer
        {
            get;
        }

        #region [====== Serialize ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_Throws_IfContentIsNull()
        {
            Serializer.Serialize(null);
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfContentIsObject()
        {
            AssertSerializedValue(Serializer.Serialize(new object()));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfContentIsInt32()
        {
            AssertSerializedValue(Serializer.Serialize(Clock.SystemClock.LocalDateAndTime().Millisecond));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfContentIsSomeCustomObject()
        {
            AssertSerializedValue(Serializer.Serialize(new SomeCustomObject()));
        }

        protected static void AssertSerializedValue(byte[] content)
        {
            Assert.IsNotNull(content);
            Assert.IsTrue(content.Length > 0);
        }

        #endregion

        #region [====== Deserialize ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfContentIsNull()
        {
            Serializer.Deserialize(null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfContentTypeIsNull()
        {
            Serializer.Deserialize(new byte[0], null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfContentCannotBeDeserialized()
        {
            Serializer.Deserialize(new byte[0], typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfContentCannotBeDeserializedToSpecifiedType()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);

            Serializer.Deserialize(serializedValue, typeof(SomeCustomObject));
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfContentTypeIsInt32()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = (int) Serializer.Deserialize(serializedValue, value.GetType());

            Assert.AreEqual(value, deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfContentTypeIsSomeCustomObject()
        {
            var value = new SomeCustomObject();
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = (SomeCustomObject) Serializer.Deserialize(serializedValue, value.GetType());

            Assert.AreNotSame(value, deserializedValue);
            Assert.AreEqual(value.Id, deserializedValue.Id);
        }

        #endregion
    }
}

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
        public void Serialize_Throws_IfInstanceIsNull()
        {
            Serializer.Serialize(null);
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsObject()
        {
            AssertSerializedValue(Serializer.Serialize(new object()));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsInt32()
        {
            AssertSerializedValue(Serializer.Serialize(Clock.Current.LocalDateAndTime().Millisecond));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsSomeCustomObject()
        {
            AssertSerializedValue(Serializer.Serialize(new SomeCustomObject()));
        }

        protected static void AssertSerializedValue(string value)
        {
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length > 0);
        }

        #endregion

        #region [====== Deserialize ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfValueIsNull()
        {
            Serializer.Deserialize(null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfTypeIsNull()
        {
            Serializer.Deserialize(string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeDeserialized()
        {
            Serializer.Deserialize(string.Empty, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeDeserializedToSpecifiedType()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);

            Serializer.Deserialize(serializedValue, typeof(SomeCustomObject));
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeIsInt32()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = (int) Serializer.Deserialize(serializedValue, value.GetType());

            Assert.AreEqual(value, deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeIsSomeCustomObject()
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

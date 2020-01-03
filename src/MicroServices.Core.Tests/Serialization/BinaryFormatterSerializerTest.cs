using System;
using System.Runtime.Serialization;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class BinaryFormatterSerializerTest
    {
        #region [====== SomeCustomObject ======]

        [Serializable]
        private sealed class SomeCustomObject
        {
            public SomeCustomObject()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id
            {
                get;
                set;
            }
        }

        #endregion

        private readonly BinaryFormatterSerializer _serializer;

        public BinaryFormatterSerializerTest()
        {
            _serializer = new BinaryFormatterSerializer();
        }

        #region [====== Serialize ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_Throws_IfInstanceIsNull()
        {
            _serializer.Serialize(null);
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsInt32()
        {
            AssertValue(_serializer.Serialize(Clock.Current.LocalDateAndTime().Millisecond));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsObject()
        {
            AssertValue(_serializer.Serialize(new object()));
        }

        [TestMethod]
        public void Serialize_ReturnsSerializedValue_IfInstanceIsSomeCustomObject()
        {
            AssertValue(_serializer.Serialize(new SomeCustomObject()));
        }

        private static void AssertValue(string value)
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
            _serializer.Deserialize(null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Deserialize_Throws_IfTypeIsNull()
        {
            _serializer.Deserialize(string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeDecoded()
        {
            _serializer.Deserialize(string.Empty, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueCannotBeConvertedToSpecifiedType()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = _serializer.Serialize(value);

            _serializer.Deserialize(serializedValue, typeof(SomeCustomObject));
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeIsEqualToSpecifiedType()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = _serializer.Serialize(value);
            var deserializedValue = (int) _serializer.Deserialize(serializedValue, value.GetType());

            Assert.AreEqual(value, deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeIsDerivedFromSpecifiedType()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = _serializer.Serialize(value);
            var deserializedValue = _serializer.Deserialize(serializedValue, typeof(object));

            Assert.AreEqual(value, deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeCanBeConvertedToSpecifiedType()
        {
            var value = Clock.Current.LocalDateAndTime().Millisecond;
            var serializedValue = _serializer.Serialize(value);
            var deserializedValue = (string) _serializer.Deserialize(serializedValue, typeof(string));

            Assert.AreEqual(value.ToString(), deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueIsSomeCustomObject()
        {
            var value = new SomeCustomObject();
            var serializedValue = _serializer.Serialize(value);
            var deserializedValue = (SomeCustomObject) _serializer.Deserialize(serializedValue, value.GetType());

            Assert.AreNotSame(value, deserializedValue);
            Assert.AreEqual(value.Id, deserializedValue.Id);
        }

        #endregion
    }
}

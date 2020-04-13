using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class BinaryFormatSerializerTest : SerializerTest<BinaryFormatSerializer>
    {
        public BinaryFormatSerializerTest()
        {
            Serializer = new BinaryFormatSerializer();
        }

        protected override BinaryFormatSerializer Serializer
        {
            get;
        }

        #region [====== Deserialize ======]

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeIsDerivedFromSpecifiedType()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = Serializer.Deserialize(serializedValue, typeof(object));

            Assert.AreEqual(value, deserializedValue);
        }

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfValueTypeCanBeConvertedToSpecifiedType()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = (string) Serializer.Deserialize(serializedValue, typeof(string));

            Assert.AreEqual(value.ToString(), deserializedValue);
        }

        #endregion
    }
}

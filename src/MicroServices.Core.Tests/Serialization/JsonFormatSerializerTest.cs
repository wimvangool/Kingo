using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class JsonFormatSerializerTest : SerializerTest<JsonFormatSerializer>
    {
        #region [====== SomeOtherObject ======]

        private sealed class SomeOtherObject
        {
            public Guid Id
            {
                get;
                set;
            }
        }

        #endregion

        public JsonFormatSerializerTest()
        {
            Serializer = new JsonFormatSerializer();
        }

        protected override JsonFormatSerializer Serializer
        {
            get;
        }

        #region [====== Deserialize ======]

        [TestMethod]
        public void Deserialize_ReturnsExpectedValue_IfContentTypeIsNotEqualToOriginalType()
        {
            var value = new SomeCustomObject();
            var serializedValue = Serializer.Serialize(value);
            var deserializedValue = (SomeOtherObject) Serializer.Deserialize(serializedValue, typeof(SomeOtherObject));

            Assert.AreEqual(value.Id, deserializedValue.Id);
        }

        #endregion
    }
}

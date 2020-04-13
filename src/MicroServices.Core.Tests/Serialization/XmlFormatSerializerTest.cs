using System.Runtime.Serialization;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Serialization
{
    [TestClass]
    public sealed class XmlFormatSerializerTest : SerializerTest<XmlFormatSerializer>
    {
        public XmlFormatSerializerTest()
        {
            Serializer = new XmlFormatSerializer();
        }

        protected override XmlFormatSerializer Serializer
        {
            get;
        }

        #region [====== Deserialize ======]

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void Deserialize_Throws_IfValueTypeIsDerivedFromSpecifiedType()
        {
            var value = Clock.SystemClock.LocalDateAndTime().Millisecond;
            var serializedValue = Serializer.Serialize(value);
            
            Serializer.Deserialize(serializedValue, typeof(object));
        }

        #endregion
    }
}

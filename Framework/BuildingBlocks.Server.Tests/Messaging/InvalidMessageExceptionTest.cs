using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
{
    [TestClass]
    public sealed class InvalidMessageExceptionTest
    {        
        [TestMethod]
        public void Exception_IsCorrectlySerializedAndDeserialized_WithBinaryFormatter()
        {
            throw new NotImplementedException();
        }

        private static InvalidMessageException CopyThroughSerialization(InvalidMessageException exception)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, exception);
                stream.Position = 0;

                return (InvalidMessageException) formatter.Deserialize(stream);
            }
        }            
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;
using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging
{
    [TestClass]
    public sealed class MessageTest
    {        
        #region [====== MessageWithoutAttributes ======]

        private sealed class MessageWithoutAttributes : Message<MessageWithoutAttributes>
        {
            internal readonly int Value;
    
            public MessageWithoutAttributes(int value)
            {
                Value = value;
            }
        }

        #endregion

        #region [====== MessageWithPrimitiveMembers ======]

        [DataContract]
        private sealed class MessageWithPrimitiveMembers : Message<MessageWithPrimitiveMembers>
        {
            [DataMember] internal readonly int Value;
            [DataMember] internal readonly List<int> Values;

            public MessageWithPrimitiveMembers(int value, IEnumerable<int> values)
            {
                Value = value;
                Values = new List<int>(values);
            }
        }

        #endregion

        #region [====== MessageWithMemberOfOwnType ======]

        [DataContract()]
        private sealed class MessageWithMemberOfOwnType : Message<MessageWithMemberOfOwnType>
        {
            [DataMember] internal readonly int Value;
            [DataMember] internal readonly MessageWithMemberOfOwnType Child;           

            public MessageWithMemberOfOwnType(int value, MessageWithMemberOfOwnType child)
            {
                Value = value;
                Child = child;
            }
        }

        #endregion        

        [TestMethod]
        [ExpectedException(typeof(InvalidDataContractException))]
        public void Copy_Throws_IfMessageLacksRequiredAttributes()
        {            
            new MessageWithoutAttributes(RandomValue()).Copy();            
        }

        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IfMessageHasNoMembersToCopy()
        {
            var message = new EmptyMessage();
            var copy = message.Copy();

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
        }

        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IsMessageSpecifiesMemberAttributes()
        {
            var values = new[] { RandomValue(), RandomValue() + 1, RandomValue() + 2 };
            var message = new MessageWithPrimitiveMembers(RandomValue(), values);
            var copy = message.Copy();

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
            Assert.AreEqual(message.Value, copy.Value);
            Assert.IsNotNull(copy.Values);
            Assert.AreEqual(3, copy.Values.Count);
            Assert.AreEqual(values[0], copy.Values[0]);
            Assert.AreEqual(values[1], copy.Values[1]);
            Assert.AreEqual(values[2], copy.Values[2]);
        }        
            
        [TestMethod]
        public void Copy_ReturnsExpectedCopy_IfMessageContainsMemberOfOwnType()
        {
            var child = new MessageWithMemberOfOwnType(RandomValue(), null);
            var message = new MessageWithMemberOfOwnType(RandomValue() + 1, child);
            var copy = message.Copy();

            Assert.IsNotNull(copy);
            Assert.AreNotSame(message, copy);
            Assert.AreEqual(message.Value, copy.Value);
            Assert.IsNotNull(copy.Child);
            Assert.AreNotSame(child, copy.Child);
            Assert.AreEqual(child.Value, copy.Child.Value);
        }        

        private static int RandomValue()
        {
            return Clock.Current.LocalDateAndTime().Millisecond;
        }
    }
}

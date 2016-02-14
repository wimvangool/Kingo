using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    public sealed partial class MessageTest
    {        
        #region [====== Equals - General Logic ======]

        [DataContract]
        private sealed class NoDataMembersMessage : Message { }

        private sealed class NoDataContractMessage : Message
        {
            [UsedImplicitly]
            private readonly int _intValue;

            [UsedImplicitly]
            private readonly string _stringValue;

            public NoDataContractMessage(int intValue, string stringValue)
            {
                _intValue = intValue;
                _stringValue = stringValue;
            }
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfOtherIsNull()
        {
            var message = new NoDataMembersMessage();

            Assert.IsFalse(message.Equals(null));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfOtherIsNotOfSameType()
        {
            var messageA = new NoDataMembersMessage();
            var messageB = new object();

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfOtherIsSameInstance()
        {
            var messageA = new NoDataMembersMessage();

            Assert.IsTrue(messageA.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsNoFields()
        {
            var messageA = new NoDataMembersMessage();
            var messageB = new NoDataMembersMessage();

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsNoDataContract_And_OtherIsSameInstance()
        {
            var message = new NoDataContractMessage(0, null);

            Assert.IsTrue(message.Equals(message));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsNoDataContract_And_FieldsHaveSameValue()
        {
            var intValue = Clock.Current.UtcDateAndTime().Millisecond;
            var stringValue = GenerateValue();

            var messageA = new NoDataContractMessage(intValue, stringValue);
            var messageB = new NoDataContractMessage(intValue, stringValue);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsNoDataContract_And_FieldsHaveDifferentValues()
        {            
            var messageA = new NoDataContractMessage(0, null);
            var messageB = new NoDataContractMessage(1, GenerateValue());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        #endregion        

        #region [====== Equals - IntFieldMessage ======]

        [DataContract]
        private sealed class IntFieldMessage : Message
        {            
            [DataMember]
            private readonly int _value;

            public IntFieldMessage(int value)
            {
                _value = value;
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsValueTypeField_And_FieldsHaveSameValue()
        {
            var messageA = new IntFieldMessage(0);
            var messageB = new IntFieldMessage(0);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageContainsValueTypeField_And_FieldsDontHaveSameValue()
        {
            var messageA = new IntFieldMessage(0);
            var messageB = new IntFieldMessage(1);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - IntPropertyMessage ======]

        [DataContract]
        private sealed class IntPropertyMessage : Message
        {            
            private readonly int _value;

            public IntPropertyMessage(int value)
            {
                _value = value;
            }

            [DataMember]
            public int Value
            {
                get { return _value; }
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsValueTypeProperty_And_FieldsHaveSameValue()
        {
            var messageA = new IntPropertyMessage(0);
            var messageB = new IntPropertyMessage(0);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageContainsValueTypeProperty_And_FieldsDontHaveSameValue()
        {
            var messageA = new IntPropertyMessage(0);
            var messageB = new IntPropertyMessage(1);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - NullableIntFieldMessage ======]

        [DataContract]
        private sealed class NullableIntFieldMessage : Message
        {
            [DataMember]
            private int? _value;

            public NullableIntFieldMessage(int? value)
            {
                _value = value;
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsNullableValueTypeField_And_FieldsAreBothNull()
        {
            var messageA = new NullableIntFieldMessage(null);
            var messageB = new NullableIntFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsNullableValueTypeField_And_FieldsHaveSameValue()
        {
            var messageA = new NullableIntFieldMessage(0);
            var messageB = new NullableIntFieldMessage(0);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageContainsNullableValueTypeField_And_FieldsDontHaveSameValue()
        {
            var messageA = new NullableIntFieldMessage(0);
            var messageB = new NullableIntFieldMessage(1);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - StringFieldMessage ======]

        [DataContract]
        private sealed class StringFieldMessage : Message
        {
            [DataMember]
            private readonly string _value;

            public StringFieldMessage(string value)
            {
                _value = value;
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsReferenceTypeField_And_FieldsAreBothNull()
        {
            var messageA = new StringFieldMessage(null);
            var messageB = new StringFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageContainsReferenceTypeField_And_FieldsHaveSameValue()
        {
            var value = GenerateValue();
            var messageA = new StringFieldMessage(value);
            var messageB = new StringFieldMessage(value);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageContainsReferenceTypeField_And_FieldsDontHaveSameValue()
        {
            var messageA = new StringFieldMessage(GenerateValue());
            var messageB = new StringFieldMessage(GenerateValue());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        private static string GenerateValue()
        {
            return Guid.NewGuid().ToString("N");
        }

        #endregion

        #region [====== Equals - EnumerableFieldMessage ======]

        [DataContract]
        private sealed class EnumerableFieldMessage : Message
        {
            [DataMember]
            private readonly IEnumerable _items;                       
 
            public EnumerableFieldMessage(IEnumerable items)
            {
                _items = items;
            }

            public static EnumerableFieldMessage WithItems(params object[] items)
            {
                return new EnumerableFieldMessage(new EnumerableStub(items));
            }
        }

        private sealed class EnumerableStub : IEnumerable
        {
            private readonly object[] _items;

            public EnumerableStub(params object[] items)
            {
                _items = items;
            }

            public IEnumerator GetEnumerator()
            {
                return _items.GetEnumerator();
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsEnumerableType_And_BothCollectionsAreNull()
        {
            var messageA = new EnumerableFieldMessage(null);
            var messageB = new EnumerableFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsEnumerableType_And_LeftIsNullAndRightIsNotNull()
        {
            var messageA = new EnumerableFieldMessage(null);
            var messageB = EnumerableFieldMessage.WithItems();

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsEnumerableType_And_LeftIsNotNullAndRightIsNull()
        {
            var messageA = EnumerableFieldMessage.WithItems();
            var messageB = new EnumerableFieldMessage(null);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsEnumerableType_And_BothLeftAndAreAreEmpty()
        {
            var messageA = EnumerableFieldMessage.WithItems();
            var messageB = EnumerableFieldMessage.WithItems();

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsEnumerableType_And_LeftHasMoreItemsThanRight()
        {
            var messageA = EnumerableFieldMessage.WithItems(new object());
            var messageB = EnumerableFieldMessage.WithItems();

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsEnumerableType_And_BothLeftAndRightContainOneItem_And_ItemsAreNotEqual()
        {
            var messageA = EnumerableFieldMessage.WithItems(new object());
            var messageB = EnumerableFieldMessage.WithItems(new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsEnumerableType_And_BothLeftAndRightContainOneItem_And_ItemsAreEqual()
        {
            var item = new object();
            var messageA = EnumerableFieldMessage.WithItems(item);
            var messageB = EnumerableFieldMessage.WithItems(item);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - ArrayFieldMessage ======]

        [DataContract]
        private sealed class ArrayFieldMessage : Message
        {
            [DataMember]
            private readonly object[] _items;

            public ArrayFieldMessage(object[] items)
            {
                _items = items;
            }

            public static ArrayFieldMessage WithItems(params object[] items)
            {
                return new ArrayFieldMessage(items);
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsArrayType_And_BothCollectionsAreNull()
        {
            var messageA = new ArrayFieldMessage(null);
            var messageB = new ArrayFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsArrayType_And_LeftIsNullAndRightIsNotNull()
        {
            var messageA = new ArrayFieldMessage(null);
            var messageB = ArrayFieldMessage.WithItems();

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsArrayType_And_LeftIsNotNullAndRightIsNull()
        {
            var messageA = ArrayFieldMessage.WithItems();
            var messageB = new ArrayFieldMessage(null);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsArrayType_And_BothLeftAndAreAreEmpty()
        {
            var messageA = ArrayFieldMessage.WithItems();
            var messageB = ArrayFieldMessage.WithItems();

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsArrayType_And_LeftHasMoreItemsThanRight()
        {
            var messageA = ArrayFieldMessage.WithItems(new object());
            var messageB = ArrayFieldMessage.WithItems();

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsArrayType_And_BothLeftAndRightContainOneItem_And_ItemsAreNotEqual()
        {
            var messageA = ArrayFieldMessage.WithItems(new object());
            var messageB = ArrayFieldMessage.WithItems(new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsArrayType_And_BothLeftAndRightContainOneItem_And_ItemsAreEqual()
        {
            var item = new object();
            var messageA = ArrayFieldMessage.WithItems(item);
            var messageB = ArrayFieldMessage.WithItems(item);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - HashtableFieldMessage ======]

        [DataContract]
        private sealed class HashtableFieldMessage : Message
        {
            [DataMember]
            private readonly Hashtable _items;            

            public HashtableFieldMessage(Hashtable items)
            {
                _items = items;
            }            

            public void Add(object key, object value)
            {
                _items.Add(key, value);
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsHashtableType_And_BothCollectionsAreNull()
        {
            var messageA = new HashtableFieldMessage(null);
            var messageB = new HashtableFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsHashtableType_And_LeftIsNullAndRightIsNotNull()
        {
            var messageA = new HashtableFieldMessage(null);
            var messageB = new HashtableFieldMessage(new Hashtable());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsHashtableType_And_LeftIsNotNullAndRightIsNull()
        {
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(null);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsHashtableType_And_BothLeftAndAreAreEmpty()
        {
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(new Hashtable());

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsHashtableType_And_LeftHasMoreItemsThanRight()
        {
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(new Hashtable());

            messageA.Add(new object(), new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsHashtableType_And_BothLeftAndRightContainOneItem_And_ItemsAreNotEqual()
        {
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(new Hashtable());

            messageA.Add(new object(), new object());
            messageB.Add(new object(), new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsHashtableType_And_BothLeftAndRightContainOneItem_And_ItemsAreEqual()
        {            
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(new Hashtable());

            var key = new object();            
            var value = new object();
            
            messageA.Add(key, value);
            messageB.Add(key, value);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsHashtableType_And_BothLeftAndRightContainTwoItems_And_ItemsAreEqual_And_ItemsWereAddedInDifferentOrder()
        {
            var messageA = new HashtableFieldMessage(new Hashtable());
            var messageB = new HashtableFieldMessage(new Hashtable());

            var keyA = new object();
            var keyB = new object();

            var valueA = new object();
            var valueB = new object();

            messageA.Add(keyA, valueA);
            messageA.Add(keyB, valueB);

            messageB.Add(keyB, valueB);
            messageB.Add(keyA, valueA);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        #endregion

        #region [====== Equals - DictionaryFieldMessage ======]

        [DataContract]
        private sealed class DictionaryFieldMessage : Message
        {
            [DataMember]
            private readonly Dictionary<int, object> _items;

            public DictionaryFieldMessage(Dictionary<int, object> items)
            {
                _items = items;
            }

            public void Add(int key, object value)
            {
                _items.Add(key, value);
            }
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsDictionaryType_And_BothCollectionsAreNull()
        {
            var messageA = new DictionaryFieldMessage(null);
            var messageB = new DictionaryFieldMessage(null);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsDictionaryType_And_LeftIsNullAndRightIsNotNull()
        {
            var messageA = new DictionaryFieldMessage(null);
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsDictionaryType_And_LeftIsNotNullAndRightIsNull()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(null);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsDictionaryType_And_BothLeftAndAreAreEmpty()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsDictionaryType_And_LeftHasMoreItemsThanRight()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            messageA.Add(0, new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMemberIsDictionaryType_And_BothLeftAndRightContainOneItem_And_ItemsAreNotEqual()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            messageA.Add(0, new object());
            messageB.Add(0, new object());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsDictionaryType_And_BothLeftAndRightContainOneItem_And_ItemsAreEqual()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            var key = 0;
            var value = new object();

            messageA.Add(key, value);
            messageB.Add(key, value);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMemberIsDictionaryType_And_BothLeftAndRightContainTwoItems_And_ItemsAreEqual_And_ItemsWereAddedInDifferentOrder()
        {
            var messageA = new DictionaryFieldMessage(new Dictionary<int, object>());
            var messageB = new DictionaryFieldMessage(new Dictionary<int, object>());

            var keyA = 0;
            var keyB = 1;

            var valueA = new object();
            var valueB = new object();

            messageA.Add(keyA, valueA);
            messageA.Add(keyB, valueB);

            messageB.Add(keyB, valueB);
            messageB.Add(keyA, valueA);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        #endregion
    }
}

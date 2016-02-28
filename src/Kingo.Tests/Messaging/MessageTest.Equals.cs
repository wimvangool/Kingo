using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Kingo.Clocks;
using Kingo.DynamicMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    public sealed partial class MessageTest
    {        
        #region [====== Equals - General Logic ======]

        private abstract class TestMessage : Message
        {
            public abstract int ExpectedHashCode();
        }

        [DataContract]
        private sealed class NoDataMembersMessage : TestMessage
        {
            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode();
            }
        }

        private sealed class BasicMessage : TestMessage
        {
            [UsedImplicitly]
            private readonly int _intValue;

            [UsedImplicitly]
            private readonly string _stringValue;

            public BasicMessage(int intValue, string stringValue)
            {
                _intValue = intValue;
                _stringValue = stringValue;
            }

            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode() ^ _intValue.GetHashCode() ^ _stringValue.GetHashCode();
            }
        }

        [Serializable]
        private sealed class SerializableMessage : TestMessage
        {
            [UsedImplicitly]
            private readonly int _intValue;

            [UsedImplicitly, NonSerialized]
            private readonly string _stringValue;

            public SerializableMessage(int intValue, string stringValue)
            {
                _intValue = intValue;
                _stringValue = stringValue;
            }

            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode() ^ _intValue.GetHashCode();
            }
        }

        [CustomFilter]
        private sealed class CustomFilteredMessage : TestMessage
        {
            [UsedImplicitly]
            private readonly int _a;

            [UsedImplicitly]
            private readonly int _b;

            public CustomFilteredMessage(int a, int b)
            {
                _a = a;
                _b = b;
            }

            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode() ^ _a.GetHashCode();
            }
        }

        private sealed class CustomFilterAttribute : MemberFilterAttribute
        {
            public override IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields)
            {
                return fields.Where(field => field.Name == "_a");
            }

            public override IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties)
            {
                return properties;
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
            var message = new BasicMessage(0, null);

            Assert.IsTrue(message.Equals(message));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsSerializable_And_SerializableFieldsHaveSameValue()
        {
            var intValue = GenerateIntValue();            

            var messageA = new SerializableMessage(intValue, GenerateStringValue());
            var messageB = new SerializableMessage(intValue, GenerateStringValue());

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsSerializable_And_SerializableFieldsHaveDifferentValue()
        {
            var intValue = GenerateIntValue();
            var stringValue = GenerateStringValue();

            var messageA = new SerializableMessage(intValue, stringValue);
            var messageB = new SerializableMessage(intValue + 1, stringValue);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsNeitherDataContractNorSerializable_And_FieldsHaveSameValue()
        {
            var intValue = GenerateIntValue();
            var stringValue = GenerateStringValue();

            var messageA = new BasicMessage(intValue, stringValue);
            var messageB = new BasicMessage(intValue, stringValue);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsNeitherDataContractNorSerializable_And_FieldsHaveDifferentValues()
        {
            var intValue = GenerateIntValue();

            var messageA = new BasicMessage(intValue, null);
            var messageB = new BasicMessage(intValue + 1, GenerateStringValue());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        } 
        
        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsCustomFiltered_And_FieldsHaveSameValue()
        {
            var intValue = GenerateIntValue();

            var messageA = new CustomFilteredMessage(intValue, intValue);
            var messageB = new CustomFilteredMessage(intValue, intValue + 1);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsCustomFiltered_And_FieldsHaveDifferentValue()
        {
            var intValue = GenerateIntValue();

            var messageA = new CustomFilteredMessage(intValue, intValue);
            var messageB = new CustomFilteredMessage(intValue + 1, intValue);

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }

        #endregion

        #region [====== GetHashCode - General Logic ======]

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageContainsNoFields()
        {
            var message = new NoDataMembersMessage();

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageIsNeitherDataContractNorSerializable()
        {
            var message = new BasicMessage(GenerateIntValue(), GenerateStringValue());

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageIsSerializable()
        {
            var message = new SerializableMessage(GenerateIntValue(), GenerateStringValue());

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
        }

        #endregion

        #region [====== Equals - IntFieldMessage ======]

        [DataContract]
        private sealed class IntFieldMessage : TestMessage
        {            
            [DataMember]
            private readonly int _value;

            public IntFieldMessage(int value)
            {
                _value = value;
            }

            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode() ^ _value.GetHashCode();
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

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageContainsValueTyeField()
        {
            var message = new IntFieldMessage(GenerateIntValue());

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
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
        private sealed class StringFieldMessage : TestMessage
        {
            [DataMember]
            private readonly string _value;

            public StringFieldMessage(string value)
            {
                _value = value;
            }

            public override int ExpectedHashCode()
            {
                return GetType().GetHashCode() ^ (_value == null ? 0 : _value.GetHashCode());
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
            var value = GenerateStringValue();
            var messageA = new StringFieldMessage(value);
            var messageB = new StringFieldMessage(value);

            Assert.IsTrue(messageA.Equals(messageB));
            Assert.IsTrue(messageB.Equals(messageA));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageContainsReferenceTypeField_And_FieldsDontHaveSameValue()
        {
            var messageA = new StringFieldMessage(GenerateStringValue());
            var messageB = new StringFieldMessage(GenerateStringValue());

            Assert.IsFalse(messageA.Equals(messageB));
            Assert.IsFalse(messageB.Equals(messageA));
        }        

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageContainsStringTypeField_And_StringIsNull()
        {
            var message = new StringFieldMessage(null);

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_ReturnsExpectedValue_IfMessageContainsStringTypeField_And_StringIsNotNull()
        {
            var message = new StringFieldMessage(GenerateStringValue());

            Assert.AreEqual(message.ExpectedHashCode(), message.GetHashCode());
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

        private static int GenerateIntValue()
        {
            return Clock.Current.UtcDateAndTime().Millisecond;
        }

        private static string GenerateStringValue()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}

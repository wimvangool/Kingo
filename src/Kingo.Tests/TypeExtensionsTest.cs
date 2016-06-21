using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class TypeExtensionsTest
    {
        #region [====== Dummy Types ======]

        private struct GenericStruct<TValue> : IEquatable<GenericStruct<TValue>> where TValue : struct
        {
            private readonly TValue _value;

            public GenericStruct(TValue value)
            {
                _value = value;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    return false;
                }
                if (obj is GenericStruct<TValue>)
                {
                    return Equals((GenericStruct<TValue>) obj);
                }
                return false;
            }

            public bool Equals(GenericStruct<TValue> other)
            {
                return _value.Equals(other._value);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            #region [====== Binary Operators ======]

            public static bool operator ==(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorEqualityCovered.Value = true;                
            }            

            public static bool operator !=(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorInequalityCovered.Value = true;                
            }            

            public static bool operator <(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorLessThanCovered.Value = true;                
            }            

            public static bool operator <=(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorLessThanOrEqualCovered.Value = true;                
            }            

            public static bool operator >(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorGreaterThanCovered.Value = true;
            }            

            public static bool operator >=(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                return _OperatorGreaterThanOrEqualCovered.Value = true;
            }

            public static GenericStruct<TValue> operator +(GenericStruct<TValue> keft, GenericStruct<TValue> right)
            {
                _OperatorAdditionCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator -(GenericStruct<TValue> keft, GenericStruct<TValue> right)
            {
                _OperatorSubtractionCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator *(GenericStruct<TValue> keft, GenericStruct<TValue> right)
            {
                _OperatorMultiplyCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator /(GenericStruct<TValue> keft, GenericStruct<TValue> right)
            {
                _OperatorDivisionCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator %(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                _OperatorModulusCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator ^(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                _OperatorExclusiveOrCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator &(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                _OperatorBitwiseAndCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator |(GenericStruct<TValue> left, GenericStruct<TValue> right)
            {
                _OperatorBitwiseOrCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator <<(GenericStruct<TValue> left, int right)
            {
                _OperatorLeftShiftCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            public static GenericStruct<TValue> operator >>(GenericStruct<TValue> left, int right)
            {
                _OperatorRightShiftCovered.Value = true;
                return new GenericStruct<TValue>();
            }

            #endregion

            #region [====== Unary Operators ======]

            public static GenericStruct<TValue> operator +(GenericStruct<TValue> value)
            {
                _OperatorUnaryPlusCovered.Value = true;
                return value;
            }

            public static GenericStruct<TValue> operator -(GenericStruct<TValue> value)
            {
                _OperatorUnaryNegationCovered.Value = true;
                return value;
            }

            public static GenericStruct<TValue> operator !(GenericStruct<TValue> value)
            {
                _OperatorLogicalNotCovered.Value = true;
                return value;
            }

            public static GenericStruct<TValue> operator ~(GenericStruct<TValue> value)
            {
                _OperatorOnesComplementCovered.Value = true;
                return value;
            }

            public static bool operator true(GenericStruct<TValue> value)
            {
                return _OperatorTrueCovered.Value = true;                
            }

            public static bool operator false(GenericStruct<TValue> value)
            {
                return _OperatorFalseCovered.Value = true;                
            }

            public static implicit operator TValue(GenericStruct<TValue> value)
            {
                _OperatorImplicitCovered.Value = true;
                return value._value;
            }

            public static explicit operator GenericStruct<TValue>(TValue value)
            {
                _OperatorExplicitCovered.Value = true;
                return new GenericStruct<TValue>(value);
            }

            #endregion
        }

        private struct EmptyStruct { }

        #endregion

        #region [====== Binary Operator Coverage ======]

        private static readonly ThreadLocal<bool> _OperatorEqualityCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorInequalityCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorLessThanCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorLessThanOrEqualCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorGreaterThanCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorGreaterThanOrEqualCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorAdditionCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorSubtractionCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorMultiplyCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorDivisionCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorModulusCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorExclusiveOrCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorBitwiseAndCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorBitwiseOrCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorLeftShiftCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorRightShiftCovered = new ThreadLocal<bool>();

        #endregion

        #region [====== Unary Operator Coverage ======]

        private static readonly ThreadLocal<bool> _OperatorUnaryPlusCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorUnaryNegationCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorLogicalNotCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorOnesComplementCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorTrueCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorFalseCovered = new ThreadLocal<bool>();

        private static readonly ThreadLocal<bool> _OperatorImplicitCovered = new ThreadLocal<bool>();
        private static readonly ThreadLocal<bool> _OperatorExplicitCovered = new ThreadLocal<bool>();

        #endregion

        private static readonly GenericStruct<int> _Instance = new GenericStruct<int>(Clocks.Clock.Current.UtcDateAndTime().Millisecond);

        #region [====== TryGetEqualityOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetEqualityOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> equalityOperator;

            TypeExtensions.TryGetEqualityOperator(null, out equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> equalityOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetEqualityOperator(out equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfEqualityOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> equalityOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetEqualityOperator(out equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfEqualityOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> equalityOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetEqualityOperator(out equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsTrue_IfEqualityOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> equalityOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetEqualityOperator(out equalityOperator));
            Assert.IsNotNull(equalityOperator);

            Assert.IsTrue(equalityOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorEqualityCovered.Value);
        }

        #endregion

        #region [====== TryGetInequalityOperator ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetInequalityOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> inequalityOperator;

            TypeExtensions.TryGetInequalityOperator(null, out inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> inequalityOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetInequalityOperator(out inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfInequalityOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> inequalityOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetInequalityOperator(out inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfInequalityOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> inequalityOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetInequalityOperator(out inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsTrue_IfInequalityOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> inequalityOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetInequalityOperator(out inequalityOperator));
            Assert.IsNotNull(inequalityOperator);

            Assert.IsTrue(inequalityOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorInequalityCovered.Value);
        }

        #endregion

        #region [====== TryGetLessThanOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetLessThanOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> lessThanOrEqualOperator;

            TypeExtensions.TryGetLessThanOperator(null, out lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetLessThanOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfLessThanOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetLessThanOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfLessThanOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLessThanOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsTrue_IfLessThanOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLessThanOperator(out lessThanOrEqualOperator));
            Assert.IsNotNull(lessThanOrEqualOperator);

            Assert.IsTrue(lessThanOrEqualOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorLessThanCovered.Value);
        }

        #endregion

        #region [====== TryGetLessThanOrEqualOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetLessThanOrEqualOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> lessThanOrEqualOperator;

            TypeExtensions.TryGetLessThanOrEqualOperator(null, out lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetLessThanOrEqualOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfLessThanOrEqualOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetLessThanOrEqualOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfLessThanOrEqualOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLessThanOrEqualOperator(out lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsTrue_IfLessThanOrEqualOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLessThanOrEqualOperator(out lessThanOrEqualOperator));
            Assert.IsNotNull(lessThanOrEqualOperator);

            Assert.IsTrue(lessThanOrEqualOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorLessThanOrEqualCovered.Value);
        }

        #endregion

        #region [====== TryGetGreaterThanOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetGreaterThanOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> greaterThanOrEqualOperator;

            TypeExtensions.TryGetGreaterThanOperator(null, out greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetGreaterThanOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfGreaterThanOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetGreaterThanOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfGreaterThanOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetGreaterThanOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsTrue_IfGreaterThanOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetGreaterThanOperator(out greaterThanOrEqualOperator));
            Assert.IsNotNull(greaterThanOrEqualOperator);

            Assert.IsTrue(greaterThanOrEqualOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorGreaterThanCovered.Value);
        }

        #endregion

        #region [====== TryGetGreaterThanOrEqualOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetGreaterThanOrEqualOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> greaterThanOrEqualOperator;

            TypeExtensions.TryGetGreaterThanOrEqualOperator(null, out greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetGreaterThanOrEqualOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfGreaterThanOrEqualOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetGreaterThanOrEqualOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfGreaterThanOrEqualOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetGreaterThanOrEqualOperator(out greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsTrue_IfGreaterThanOrEqualOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetGreaterThanOrEqualOperator(out greaterThanOrEqualOperator));
            Assert.IsNotNull(greaterThanOrEqualOperator);

            Assert.IsTrue(greaterThanOrEqualOperator.Invoke(_Instance, _Instance));
            Assert.IsTrue(_OperatorGreaterThanOrEqualCovered.Value);
        }

        #endregion

        #region [====== TryGetAdditionOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetAdditionOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> additionOperator;

            TypeExtensions.TryGetAdditionOperator(null, out additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> additionOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetAdditionOperator(out additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfAdditionOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> additionOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetAdditionOperator(out additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfAdditionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> additionOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetAdditionOperator(out additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsTrue_IfAdditionOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> additionOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetAdditionOperator(out additionOperator));
            Assert.IsNotNull(additionOperator);

            additionOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorAdditionCovered.Value);
        }

        #endregion

        #region [====== TryGetSubtractionOperator ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetSubtractionOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> subtractionOperator;

            TypeExtensions.TryGetSubtractionOperator(null, out subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> subtractionOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetSubtractionOperator(out subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfSubtractionOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> subtractionOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetSubtractionOperator(out subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfSubtractionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> subtractionOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetSubtractionOperator(out subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsTrue_IfSubtractionOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> subtractionOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetSubtractionOperator(out subtractionOperator));
            Assert.IsNotNull(subtractionOperator);

            subtractionOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorSubtractionCovered.Value);
        }

        #endregion

        #region [====== TryGetMultiplyOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetMultiplyOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> multiplyOperator;

            TypeExtensions.TryGetMultiplyOperator(null, out multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> multiplyOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetMultiplyOperator(out multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfMultiplyOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> multiplyOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetMultiplyOperator(out multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfMultiplyOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> multiplyOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetMultiplyOperator(out multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsTrue_IfMultiplyOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> multiplyOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetMultiplyOperator(out multiplyOperator));
            Assert.IsNotNull(multiplyOperator);

            multiplyOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorMultiplyCovered.Value);
        }

        #endregion

        #region [====== TryGetDivisionOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetDivisionOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> divisionOperator;

            TypeExtensions.TryGetDivisionOperator(null, out divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> divisionOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetDivisionOperator(out divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfDivisionOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> divisionOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetDivisionOperator(out divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfDivisionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> divisionOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetDivisionOperator(out divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsTrue_IfDivisionOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> divisionOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetDivisionOperator(out divisionOperator));
            Assert.IsNotNull(divisionOperator);

            divisionOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorDivisionCovered.Value);
        }

        #endregion

        #region [====== TryGetModulusOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetModulusOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> modulusOperator;

            TypeExtensions.TryGetModulusOperator(null, out modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> modulusOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetModulusOperator(out modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfModulusOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> modulusOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetModulusOperator(out modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfModulusOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> modulusOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetModulusOperator(out modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsTrue_IfModulusOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> modulusOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetModulusOperator(out modulusOperator));
            Assert.IsNotNull(modulusOperator);

            modulusOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorModulusCovered.Value);
        }

        #endregion

        #region [====== TryGetExclusiveOrOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetExclusiveOrOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> exclusiveOrOperator;

            TypeExtensions.TryGetExclusiveOrOperator(null, out exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> exclusiveOrOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetExclusiveOrOperator(out exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfExclusiveOrOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> exclusiveOrOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetExclusiveOrOperator(out exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfExclusiveOrOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> exclusiveOrOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetExclusiveOrOperator(out exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsTrue_IfExclusiveOrOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> exclusiveOrOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetExclusiveOrOperator(out exclusiveOrOperator));
            Assert.IsNotNull(exclusiveOrOperator);

            exclusiveOrOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorExclusiveOrCovered.Value);
        }

        #endregion

        #region [====== TryGetBitwiseAndOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetBitwiseAndOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> bitwiseAndOperator;

            TypeExtensions.TryGetBitwiseAndOperator(null, out bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseAndOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetBitwiseAndOperator(out bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfBitwiseAndOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> bitwiseAndOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetBitwiseAndOperator(out bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfBitwiseAndOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseAndOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetBitwiseAndOperator(out bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsTrue_IfBitwiseAndOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> bitwiseAndOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetBitwiseAndOperator(out bitwiseAndOperator));
            Assert.IsNotNull(bitwiseAndOperator);

            bitwiseAndOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorBitwiseAndCovered.Value);
        }

        #endregion

        #region [====== TryGetBitwiseOrOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetBitwiseOrOperator_Throws_IfTypeIsNull()
        {
            Func<object, object, bool> bitwiseOrOperator;

            TypeExtensions.TryGetBitwiseOrOperator(null, out bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseOrOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetBitwiseOrOperator(out bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfBitwiseOrOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct, bool> bitwiseOrOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetBitwiseOrOperator(out bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfBitwiseOrOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseOrOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetBitwiseOrOperator(out bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsTrue_IfBitwiseOrOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> bitwiseOrOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetBitwiseOrOperator(out bitwiseOrOperator));
            Assert.IsNotNull(bitwiseOrOperator);

            bitwiseOrOperator.Invoke(_Instance, _Instance);

            Assert.IsTrue(_OperatorBitwiseOrCovered.Value);
        }

        #endregion

        #region [====== TryGetLeftShiftOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetLeftShiftOperator_Throws_IfTypeIsNull()
        {
            Func<object, int, bool> leftShiftOperator;

            TypeExtensions.TryGetLeftShiftOperator(null, out leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, int, bool> leftShiftOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetLeftShiftOperator(out leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfLeftShiftOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, int, bool> leftShiftOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetLeftShiftOperator(out leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfLeftShiftOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, int, bool> leftShiftOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLeftShiftOperator(out leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsTrue_IfLeftShiftOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, int, GenericStruct<int>> leftShiftOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLeftShiftOperator(out leftShiftOperator));
            Assert.IsNotNull(leftShiftOperator);

            leftShiftOperator.Invoke(_Instance, 0);

            Assert.IsTrue(_OperatorLeftShiftCovered.Value);
        }

        #endregion

        #region [====== TryGetRightShiftOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetRightShiftOperator_Throws_IfTypeIsNull()
        {
            Func<object, int, bool> rightShiftOperator;

            TypeExtensions.TryGetRightShiftOperator(null, out rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, int, bool> rightShiftOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetRightShiftOperator(out rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfRightShiftOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, int, bool> rightShiftOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetRightShiftOperator(out rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfRightShiftOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, int, bool> rightShiftOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetRightShiftOperator(out rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsTrue_IfRightShiftOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, int, GenericStruct<int>> rightShiftOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetRightShiftOperator(out rightShiftOperator));
            Assert.IsNotNull(rightShiftOperator);

            rightShiftOperator.Invoke(_Instance, 0);

            Assert.IsTrue(_OperatorRightShiftCovered.Value);
        }

        #endregion

        #region [====== TryGetUnaryPlusOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetUnaryPlusOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> unaryPlusOperator;

            TypeExtensions.TryGetUnaryPlusOperator(null, out unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>> unaryPlusOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetUnaryPlusOperator(out unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfUnaryPlusOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct> unaryPlusOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetUnaryPlusOperator(out unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfUnaryPlusOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> unaryPlusOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetUnaryPlusOperator(out unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsTrue_IfUnaryPlusOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>> unaryPlusOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetUnaryPlusOperator(out unaryPlusOperator));
            Assert.IsNotNull(unaryPlusOperator);

            unaryPlusOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorUnaryPlusCovered.Value);
        }

        #endregion

        #region [====== TryGetUnaryNegationOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetUnaryNegationOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> unaryNegationOperator;

            TypeExtensions.TryGetUnaryNegationOperator(null, out unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>> unaryNegationOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetUnaryNegationOperator(out unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfUnaryNegationOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct> unaryNegationOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetUnaryNegationOperator(out unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfUnaryNegationOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> unaryNegationOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetUnaryNegationOperator(out unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsTrue_IfUnaryNegationOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>> unaryNegationOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetUnaryNegationOperator(out unaryNegationOperator));
            Assert.IsNotNull(unaryNegationOperator);

            unaryNegationOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorUnaryNegationCovered.Value);
        }

        #endregion

        #region [====== TryGetLogicalNotOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetLogicalNotOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> logicalNotOperator;

            TypeExtensions.TryGetLogicalNotOperator(null, out logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>> logicalNotOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetLogicalNotOperator(out logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfLogicalNotOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct> logicalNotOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetLogicalNotOperator(out logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfLogicalNotOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> logicalNotOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLogicalNotOperator(out logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsTrue_IfLogicalNotOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>> logicalNotOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLogicalNotOperator(out logicalNotOperator));
            Assert.IsNotNull(logicalNotOperator);

            logicalNotOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorLogicalNotCovered.Value);
        }

        #endregion

        #region [====== TryGetOnesComplementOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetOnesComplementOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> onesComplementOperator;

            TypeExtensions.TryGetOnesComplementOperator(null, out onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, GenericStruct<int>> onesComplementOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetOnesComplementOperator(out onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfOnesComplementOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, EmptyStruct> onesComplementOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetOnesComplementOperator(out onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfOnesComplementOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> onesComplementOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetOnesComplementOperator(out onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsTrue_IfOnesComplementOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, GenericStruct<int>> onesComplementOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetOnesComplementOperator(out onesComplementOperator));
            Assert.IsNotNull(onesComplementOperator);

            onesComplementOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorOnesComplementCovered.Value);
        }

        #endregion

        #region [====== TryGetTrueOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetTrueOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> trueOperator;

            TypeExtensions.TryGetTrueOperator(null, out trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, bool> trueOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetTrueOperator(out trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTrueOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, bool> trueOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetTrueOperator(out trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTrueOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> trueOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetTrueOperator(out trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsTrue_IfTrueOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, bool> trueOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetTrueOperator(out trueOperator));
            Assert.IsNotNull(trueOperator);

            trueOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorTrueCovered.Value);
        }

        #endregion

        #region [====== TryGetFalseOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetFalseOperator_Throws_IfTypeIsNull()
        {
            Func<object, bool> falseOperator;

            TypeExtensions.TryGetFalseOperator(null, out falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, bool> falseOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetFalseOperator(out falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfFalseOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, bool> falseOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetFalseOperator(out falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfFalseOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> falseOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetFalseOperator(out falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsTrue_IfFalseOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, bool> falseOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetFalseOperator(out falseOperator));
            Assert.IsNotNull(falseOperator);

            falseOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorTrueCovered.Value);
        }

        #endregion

        #region [====== TryGetImplicitOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetImplicitOperator_Throws_IfTypeIsNull()
        {
            Func<object, object> implicitOperator;

            TypeExtensions.TryGetImplicitOperator(null, out implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<GenericStruct<int>, int> implicitOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetImplicitOperator(out implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfImplicitOperatorIsNotDefinedWithinType()
        {
            Func<EmptyStruct, int> implicitOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetImplicitOperator(out implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfImplicitOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<GenericStruct<int>, bool> implicitOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetImplicitOperator(out implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsTrue_IfImplicitOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<GenericStruct<int>, int> implicitOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetImplicitOperator(out implicitOperator));
            Assert.IsNotNull(implicitOperator);

            implicitOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorImplicitCovered.Value);
        }

        #endregion

        #region [====== TryGetExplicitOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetExplicitOperator_Throws_IfTypeIsNull()
        {
            Func<object, object> explicitOperator;

            TypeExtensions.TryGetExplicitOperator(null, out explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Func<int, GenericStruct<int>> explicitOperator;

            Assert.IsFalse(typeof(GenericStruct<>).TryGetExplicitOperator(out explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfExplicitOperatorIsNotDefinedWithinType()
        {
            Func<int, EmptyStruct> explicitOperator;

            Assert.IsFalse(typeof(EmptyStruct).TryGetExplicitOperator(out explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfExplicitOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Func<bool, GenericStruct<int>> explicitOperator;

            Assert.IsFalse(typeof(GenericStruct<long>).TryGetExplicitOperator(out explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsTrue_IfExplicitOperatorIsDefined_And_ArgumentsMatch()
        {
            Func<int, GenericStruct<int>> explicitOperator;

            Assert.IsTrue(typeof(GenericStruct<int>).TryGetExplicitOperator(out explicitOperator));
            Assert.IsNotNull(explicitOperator);

            explicitOperator.Invoke(0);

            Assert.IsTrue(_OperatorExplicitCovered.Value);
        }

        #endregion
    }
}

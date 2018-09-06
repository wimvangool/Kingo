using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed partial class TypeExtensionsTest
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

            public bool Equals(GenericStruct<TValue> other) =>
             _value.Equals(other._value);

            public override int GetHashCode() =>
             _value.GetHashCode();

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
            TypeExtensions.TryGetEqualityOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {            
            Assert.IsFalse(typeof(GenericStruct<>).TryGetEqualityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfEqualityOperatorIsNotDefinedWithinType()
        {            
            Assert.IsFalse(typeof(EmptyStruct).TryGetEqualityOperator(out Func<EmptyStruct, EmptyStruct, bool>  equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsFalse_IfEqualityOperatorIsDefined_But_ArgumentsDoNotMatch()
        {            
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetEqualityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  equalityOperator));
            Assert.IsNull(equalityOperator);
        }

        [TestMethod]
        public void TryGetEqualityOperator_ReturnsTrue_IfEqualityOperatorIsDefined_And_ArgumentsMatch()
        {            
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetEqualityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  equalityOperator));
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
            TypeExtensions.TryGetInequalityOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {            
            Assert.IsFalse(typeof(GenericStruct<>).TryGetInequalityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfInequalityOperatorIsNotDefinedWithinType()
        {            
            Assert.IsFalse(typeof(EmptyStruct).TryGetInequalityOperator(out Func<EmptyStruct, EmptyStruct, bool>  inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsFalse_IfInequalityOperatorIsDefined_But_ArgumentsDoNotMatch()
        {            
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetInequalityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  inequalityOperator));
            Assert.IsNull(inequalityOperator);
        }

        [TestMethod]
        public void TryGetInequalityOperator_ReturnsTrue_IfInequalityOperatorIsDefined_And_ArgumentsMatch()
        {            
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetInequalityOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  inequalityOperator));
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
            TypeExtensions.TryGetLessThanOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {            
            Assert.IsFalse(typeof(GenericStruct<>).TryGetLessThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfLessThanOperatorIsNotDefinedWithinType()
        {            
            Assert.IsFalse(typeof(EmptyStruct).TryGetLessThanOperator(out Func<EmptyStruct, EmptyStruct, bool>  lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsFalse_IfLessThanOperatorIsDefined_But_ArgumentsDoNotMatch()
        {            
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLessThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool>  lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOperator_ReturnsTrue_IfLessThanOperatorIsDefined_And_ArgumentsMatch()
        {            
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLessThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator));
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
            TypeExtensions.TryGetLessThanOrEqualOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {            
            Assert.IsFalse(typeof(GenericStruct<>).TryGetLessThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfLessThanOrEqualOperatorIsNotDefinedWithinType()
        {            
            Assert.IsFalse(typeof(EmptyStruct).TryGetLessThanOrEqualOperator(out Func<EmptyStruct, EmptyStruct, bool> lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsFalse_IfLessThanOrEqualOperatorIsDefined_But_ArgumentsDoNotMatch()
        {            
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLessThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator));
            Assert.IsNull(lessThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetLessThanOrEqualOperator_ReturnsTrue_IfLessThanOrEqualOperatorIsDefined_And_ArgumentsMatch()
        {            
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLessThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> lessThanOrEqualOperator));
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
            TypeExtensions.TryGetGreaterThanOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {            
            Assert.IsFalse(typeof(GenericStruct<>).TryGetGreaterThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfGreaterThanOperatorIsNotDefinedWithinType()
        {            
            Assert.IsFalse(typeof(EmptyStruct).TryGetGreaterThanOperator(out Func<EmptyStruct, EmptyStruct, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsFalse_IfGreaterThanOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetGreaterThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOperator_ReturnsTrue_IfGreaterThanOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetGreaterThanOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
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
            TypeExtensions.TryGetGreaterThanOrEqualOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetGreaterThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfGreaterThanOrEqualOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetGreaterThanOrEqualOperator(out Func<EmptyStruct, EmptyStruct, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsFalse_IfGreaterThanOrEqualOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetGreaterThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
            Assert.IsNull(greaterThanOrEqualOperator);
        }

        [TestMethod]
        public void TryGetGreaterThanOrEqualOperator_ReturnsTrue_IfGreaterThanOrEqualOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetGreaterThanOrEqualOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> greaterThanOrEqualOperator));
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
            TypeExtensions.TryGetAdditionOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetAdditionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfAdditionOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetAdditionOperator(out Func<EmptyStruct, EmptyStruct, bool> additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsFalse_IfAdditionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetAdditionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> additionOperator));
            Assert.IsNull(additionOperator);
        }

        [TestMethod]
        public void TryGetAdditionOperator_ReturnsTrue_IfAdditionOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetAdditionOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> additionOperator));
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
            TypeExtensions.TryGetSubtractionOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetSubtractionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfSubtractionOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetSubtractionOperator(out Func<EmptyStruct, EmptyStruct, bool> subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsFalse_IfSubtractionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetSubtractionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> subtractionOperator));
            Assert.IsNull(subtractionOperator);
        }

        [TestMethod]
        public void TryGetSubtractionOperator_ReturnsTrue_IfSubtractionOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetSubtractionOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> subtractionOperator));
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
            TypeExtensions.TryGetMultiplyOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetMultiplyOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfMultiplyOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetMultiplyOperator(out Func<EmptyStruct, EmptyStruct, bool> multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsFalse_IfMultiplyOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetMultiplyOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> multiplyOperator));
            Assert.IsNull(multiplyOperator);
        }

        [TestMethod]
        public void TryGetMultiplyOperator_ReturnsTrue_IfMultiplyOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetMultiplyOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> multiplyOperator));
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
            TypeExtensions.TryGetDivisionOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetDivisionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfDivisionOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetDivisionOperator(out Func<EmptyStruct, EmptyStruct, bool> divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsFalse_IfDivisionOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetDivisionOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> divisionOperator));
            Assert.IsNull(divisionOperator);
        }

        [TestMethod]
        public void TryGetDivisionOperator_ReturnsTrue_IfDivisionOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetDivisionOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> divisionOperator));
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
            TypeExtensions.TryGetModulusOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetModulusOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfModulusOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetModulusOperator(out Func<EmptyStruct, EmptyStruct, bool> modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsFalse_IfModulusOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetModulusOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> modulusOperator));
            Assert.IsNull(modulusOperator);
        }

        [TestMethod]
        public void TryGetModulusOperator_ReturnsTrue_IfModulusOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetModulusOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> modulusOperator));
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
            TypeExtensions.TryGetExclusiveOrOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetExclusiveOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfExclusiveOrOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetExclusiveOrOperator(out Func<EmptyStruct, EmptyStruct, bool> exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsFalse_IfExclusiveOrOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetExclusiveOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> exclusiveOrOperator));
            Assert.IsNull(exclusiveOrOperator);
        }

        [TestMethod]
        public void TryGetExclusiveOrOperator_ReturnsTrue_IfExclusiveOrOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetExclusiveOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> exclusiveOrOperator));
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
            TypeExtensions.TryGetBitwiseAndOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetBitwiseAndOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfBitwiseAndOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetBitwiseAndOperator(out Func<EmptyStruct, EmptyStruct, bool> bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsFalse_IfBitwiseAndOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetBitwiseAndOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseAndOperator));
            Assert.IsNull(bitwiseAndOperator);
        }

        [TestMethod]
        public void TryGetBitwiseAndOperator_ReturnsTrue_IfBitwiseAndOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetBitwiseAndOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> bitwiseAndOperator));
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
            TypeExtensions.TryGetBitwiseOrOperator(null, out Func<object, object, bool> _);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetBitwiseOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfBitwiseOrOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetBitwiseOrOperator(out Func<EmptyStruct, EmptyStruct, bool> bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsFalse_IfBitwiseOrOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetBitwiseOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, bool> bitwiseOrOperator));
            Assert.IsNull(bitwiseOrOperator);
        }

        [TestMethod]
        public void TryGetBitwiseOrOperator_ReturnsTrue_IfBitwiseOrOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetBitwiseOrOperator(out Func<GenericStruct<int>, GenericStruct<int>, GenericStruct<int>> bitwiseOrOperator));
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
            TypeExtensions.TryGetLeftShiftOperator(null, out Func<object, int, bool> _);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetLeftShiftOperator(out Func<GenericStruct<int>, int, bool> leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfLeftShiftOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetLeftShiftOperator(out Func<EmptyStruct, int, bool> leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsFalse_IfLeftShiftOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLeftShiftOperator(out Func<GenericStruct<int>, int, bool> leftShiftOperator));
            Assert.IsNull(leftShiftOperator);
        }

        [TestMethod]
        public void TryGetLeftShiftOperator_ReturnsTrue_IfLeftShiftOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLeftShiftOperator(out Func<GenericStruct<int>, int, GenericStruct<int>> leftShiftOperator));
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
            TypeExtensions.TryGetRightShiftOperator(null, out Func<object, int, bool> _);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetRightShiftOperator(out Func<GenericStruct<int>, int, bool> rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfRightShiftOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetRightShiftOperator(out Func<EmptyStruct, int, bool> rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsFalse_IfRightShiftOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetRightShiftOperator(out Func<GenericStruct<int>, int, bool> rightShiftOperator));
            Assert.IsNull(rightShiftOperator);
        }

        [TestMethod]
        public void TryGetRightShiftOperator_ReturnsTrue_IfRightShiftOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetRightShiftOperator(out Func<GenericStruct<int>, int, GenericStruct<int>> rightShiftOperator));
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
            TypeExtensions.TryGetUnaryPlusOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetUnaryPlusOperator(out Func<GenericStruct<int>, GenericStruct<int>> unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfUnaryPlusOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetUnaryPlusOperator(out Func<EmptyStruct, EmptyStruct> unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsFalse_IfUnaryPlusOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetUnaryPlusOperator(out Func<GenericStruct<int>, bool> unaryPlusOperator));
            Assert.IsNull(unaryPlusOperator);
        }

        [TestMethod]
        public void TryGetUnaryPlusOperator_ReturnsTrue_IfUnaryPlusOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetUnaryPlusOperator(out Func<GenericStruct<int>, GenericStruct<int>> unaryPlusOperator));
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
            TypeExtensions.TryGetUnaryNegationOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetUnaryNegationOperator(out Func<GenericStruct<int>, GenericStruct<int>> unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfUnaryNegationOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetUnaryNegationOperator(out Func<EmptyStruct, EmptyStruct> unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsFalse_IfUnaryNegationOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetUnaryNegationOperator(out Func<GenericStruct<int>, bool> unaryNegationOperator));
            Assert.IsNull(unaryNegationOperator);
        }

        [TestMethod]
        public void TryGetUnaryNegationOperator_ReturnsTrue_IfUnaryNegationOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetUnaryNegationOperator(out Func<GenericStruct<int>, GenericStruct<int>> unaryNegationOperator));
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
            TypeExtensions.TryGetLogicalNotOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetLogicalNotOperator(out Func<GenericStruct<int>, GenericStruct<int>> logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfLogicalNotOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetLogicalNotOperator(out Func<EmptyStruct, EmptyStruct> logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsFalse_IfLogicalNotOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetLogicalNotOperator(out Func<GenericStruct<int>, bool> logicalNotOperator));
            Assert.IsNull(logicalNotOperator);
        }

        [TestMethod]
        public void TryGetLogicalNotOperator_ReturnsTrue_IfLogicalNotOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetLogicalNotOperator(out Func<GenericStruct<int>, GenericStruct<int>> logicalNotOperator));
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
            TypeExtensions.TryGetOnesComplementOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetOnesComplementOperator(out Func<GenericStruct<int>, GenericStruct<int>> onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfOnesComplementOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetOnesComplementOperator(out Func<EmptyStruct, EmptyStruct> onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsFalse_IfOnesComplementOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetOnesComplementOperator(out Func<GenericStruct<int>, bool> onesComplementOperator));
            Assert.IsNull(onesComplementOperator);
        }

        [TestMethod]
        public void TryGetOnesComplementOperator_ReturnsTrue_IfOnesComplementOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetOnesComplementOperator(out Func<GenericStruct<int>, GenericStruct<int>> onesComplementOperator));
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
            TypeExtensions.TryGetTrueOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetTrueOperator(out Func<GenericStruct<int>, bool> trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTrueOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetTrueOperator(out Func<EmptyStruct, bool> trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsFalse_IfTrueOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetTrueOperator(out Func<GenericStruct<int>, bool> trueOperator));
            Assert.IsNull(trueOperator);
        }

        [TestMethod]
        public void TryGetTrueOperator_ReturnsTrue_IfTrueOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetTrueOperator(out Func<GenericStruct<int>, bool> trueOperator));
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
            TypeExtensions.TryGetFalseOperator(null, out Func<object, bool> _);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetFalseOperator(out Func<GenericStruct<int>, bool> falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfFalseOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetFalseOperator(out Func<EmptyStruct, bool> falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsFalse_IfFalseOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetFalseOperator(out Func<GenericStruct<int>, bool> falseOperator));
            Assert.IsNull(falseOperator);
        }

        [TestMethod]
        public void TryGetFalseOperator_ReturnsTrue_IfFalseOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetFalseOperator(out Func<GenericStruct<int>, bool> falseOperator));
            Assert.IsNotNull(falseOperator);

            falseOperator.Invoke(_Instance);

            Assert.IsTrue(_OperatorFalseCovered.Value);
        }

        #endregion

        #region [====== TryGetImplicitOperator ======]            

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetImplicitOperator_Throws_IfTypeIsNull()
        {
            TypeExtensions.TryGetImplicitOperator(null, out Func<object, object> _);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetImplicitOperator(out Func<GenericStruct<int>, int> implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfImplicitOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetImplicitOperator(out Func<EmptyStruct, int> implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsFalse_IfImplicitOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetImplicitOperator(out Func<GenericStruct<int>, bool> implicitOperator));
            Assert.IsNull(implicitOperator);
        }

        [TestMethod]
        public void TryGetImplicitOperator_ReturnsTrue_IfImplicitOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetImplicitOperator(out Func<GenericStruct<int>, int> implicitOperator));
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
            TypeExtensions.TryGetExplicitOperator(null, out Func<object, object> _);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfTypeIsOpenGenericType()
        {
            Assert.IsFalse(typeof(GenericStruct<>).TryGetExplicitOperator(out Func<int, GenericStruct<int>> explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfExplicitOperatorIsNotDefinedWithinType()
        {
            Assert.IsFalse(typeof(EmptyStruct).TryGetExplicitOperator(out Func<int, EmptyStruct> explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsFalse_IfExplicitOperatorIsDefined_But_ArgumentsDoNotMatch()
        {
            Assert.IsFalse(typeof(GenericStruct<long>).TryGetExplicitOperator(out Func<bool, GenericStruct<int>> explicitOperator));
            Assert.IsNull(explicitOperator);
        }

        [TestMethod]
        public void TryGetExplicitOperator_ReturnsTrue_IfExplicitOperatorIsDefined_And_ArgumentsMatch()
        {
            Assert.IsTrue(typeof(GenericStruct<int>).TryGetExplicitOperator(out Func<int, GenericStruct<int>> explicitOperator));
            Assert.IsNotNull(explicitOperator);

            explicitOperator.Invoke(0);

            Assert.IsTrue(_OperatorExplicitCovered.Value);
        }

        #endregion        
    }
}

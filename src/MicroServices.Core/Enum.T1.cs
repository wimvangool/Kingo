using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Kingo.Ensure;

namespace Kingo
{
    /// <summary>
    /// Contains several generic operators that can be used for Enum types.
    /// </summary>
    /// <typeparam name="TEnum">Type of the Enum.</typeparam>
    public static class Enum<TEnum>
    {
        #region [====== Operator ======]

        private sealed class Operator
        {
            private readonly Func<TEnum, TEnum, TEnum> _operator;

            public Operator(Func<Expression, Expression, Expression> operatorFactory)
            {
                _operator = MakeBinaryOperator(operatorFactory);
            }

            public TEnum Invoke(TEnum x, TEnum y) =>
                _operator.Invoke(x, y);

            public TEnum Invoke(params TEnum[] values) =>
                Invoke(IsNotNull(values, nameof(values)).AsEnumerable());

            public TEnum Invoke(IEnumerable<TEnum> values)
            {
                var result = default(TEnum);

                foreach (var value in IsNotNull(values, nameof(values)))
                {
                    result = _operator.Invoke(result, value);
                }
                return result;
            }
        }

        private static Func<TEnum, TEnum, TEnum> MakeBinaryOperator(Func<Expression, Expression, Expression> operatorFactory)
        {
            var integerType = Enum.GetUnderlyingType(typeof(TEnum));
            var xParameter = Expression.Parameter(typeof(TEnum), "x");
            var yParameter = Expression.Parameter(typeof(TEnum), "y");
            var x = Expression.Convert(xParameter, integerType);
            var y = Expression.Convert(yParameter, integerType);
            var z = Expression.Convert(operatorFactory.Invoke(x, y), typeof(TEnum));

            return Expression.Lambda<Func<TEnum, TEnum, TEnum>>(z, xParameter, yParameter).Compile();
        }

        #endregion

        /// <summary>
        /// Returns an Enum value where are values of the Enum are bitwise OR-ed into a single value.
        /// </summary>
        /// <returns>A bitwise OR-ed value of all defined values.</returns>
        public static TEnum AllValuesCombined() =>
             Or(AllValues());

        /// <summary>
        /// Returns all declared values of an enum.
        /// </summary>
        /// <returns>A collection of enum values.</returns>
        public static IEnumerable<TEnum> AllValues() =>
             Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        /// <summary>
        /// Determines whether or not all bits of <paramref name="value"/> are set in <paramref name="compositeValue"/>.
        /// </summary>
        /// <param name="value">The bits to check.</param>
        /// <param name="compositeValue">The collection of bits that is checked.</param>
        /// <returns>
        /// <c>true</c> if all bits of <paramref name="value"/> are set in <paramref name="compositeValue"/>; otherwise <c>false</c>.
        /// </returns>
        public static bool IsDefined(TEnum value, TEnum compositeValue) =>
             And(value, compositeValue).Equals(value);

        #region [====== Or ======]

        private static readonly Operator _OrOperator = new Operator(Expression.Or);        
        
        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>A value where the bits of <paramref name="x"/> and <paramref name="y"/> have been OR-ed.</returns>        
        public static TEnum Or(TEnum x, TEnum y) =>
             _OrOperator.Invoke(x, y);

        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been OR-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Or(params TEnum[] values) =>
            _OrOperator.Invoke(values);

        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>        
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been OR-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Or(IEnumerable<TEnum> values) =>
            _OrOperator.Invoke(values);

        #endregion

        #region [====== And ======]

        private static readonly Operator _AndOperator = new Operator(Expression.And);

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>A value where the bits of <paramref name="x"/> and <paramref name="y"/> have been AND-ed.</returns>        
        public static TEnum And(TEnum x, TEnum y) =>
             _AndOperator.Invoke(x, y);

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum And(params TEnum[] values) =>
            _AndOperator.Invoke(values);

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>        
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum And(IEnumerable<TEnum> values) =>
            _AndOperator.Invoke(values);

        #endregion

        #region [====== Xor ======]

        private static readonly Operator _XorOperator = new Operator(Expression.ExclusiveOr);

        /// <summary>
        /// Performs a binary XOR-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>A value where the bits of <paramref name="x"/> and <paramref name="y"/> have been AND-ed.</returns>        
        public static TEnum Xor(TEnum x, TEnum y) =>
            _XorOperator.Invoke(x, y);

        /// <summary>
        /// Performs a binary XOR-operation on the specified Enum types.
        /// </summary>
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Xor(params TEnum[] values) =>
            _XorOperator.Invoke(values);

        /// <summary>
        /// Performs a binary XOR-operation on the specified Enum types.
        /// </summary>        
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Xor(IEnumerable<TEnum> values) =>
            _XorOperator.Invoke(values);

        #endregion
    }
}

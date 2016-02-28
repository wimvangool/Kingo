using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo
{
    /// <summary>
    /// Contains several generic operators that can be used for Enum types.
    /// </summary>
    /// <typeparam name="TEnum">Type of the Enum</typeparam>
    public static class EnumOperators<TEnum>
    {
        #region [====== Or ======]

        private static readonly Func<TEnum, TEnum, TEnum> _BinaryOrOperator = MakeBinaryOperator(Expression.Or);        

        /// <summary>
        /// Returns an Enum value where are values of the Enum are bitwise OR-ed into a single value.
        /// </summary>
        /// <returns>A bitwise OR-ed value of all defined values.</returns>
        public static TEnum AllValuesCombined()
        {
            return Or(Enum.GetValues(typeof(TEnum)).Cast<TEnum>());
        }

        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>A value where the bits of <paramref name="x"/> and <paramref name="y"/> have been OR-ed.</returns>        
        public static TEnum Or(TEnum x, TEnum y)
        {
            return _BinaryOrOperator.Invoke(x, y);
        }

        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been OR-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Or(TEnum x, TEnum y, params TEnum[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var result = _BinaryOrOperator.Invoke(x, y);

            foreach (var value in values)
            {
                result = _BinaryOrOperator.Invoke(result, value);
            }
            return result;
        }

        /// <summary>
        /// Performs a binary OR-operation on the specified Enum types.
        /// </summary>        
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been OR-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum Or(IEnumerable<TEnum> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var result = default(TEnum);

            foreach (var value in values)
            {
                result = _BinaryOrOperator.Invoke(result, value);
            }
            return result;
        }

        #endregion

        #region [====== And ======]

        private static readonly Func<TEnum, TEnum, TEnum> _BinaryAndOperator = MakeBinaryOperator(Expression.And);        

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>A value where the bits of <paramref name="x"/> and <paramref name="y"/> have been AND-ed.</returns>        
        public static TEnum And(TEnum x, TEnum y)
        {
            return _BinaryAndOperator.Invoke(x, y);
        }

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum And(TEnum x, TEnum y, params TEnum[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var result = _BinaryOrOperator.Invoke(x, y);

            foreach (var value in values)
            {
                result = _BinaryOrOperator.Invoke(result, value);
            }
            return result;
        }

        /// <summary>
        /// Performs a binary AND-operation on the specified Enum types.
        /// </summary>        
        /// <param name="values">A collection of Enum values.</param>
        /// <returns>A value where the bits of the specified <paramref name="values"/> have been AND-ed.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        public static TEnum And(IEnumerable<TEnum> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var result = default(TEnum);

            foreach (var value in values)
            {
                result = _BinaryOrOperator.Invoke(result, value);
            }
            return result;
        }

        #endregion

        private static Func<TEnum, TEnum, TEnum> MakeBinaryOperator(Func<Expression, Expression, Expression> operatorFactory)
        {
            var integerType = Enum.GetUnderlyingType(typeof(TEnum));
            var xParameter = Expression.Parameter(typeof(TEnum), "x");
            var yParameter = Expression.Parameter(typeof(TEnum), "y");

            var binaryOrExpression =
                Expression.Lambda<Func<TEnum, TEnum, TEnum>>(
                    Expression.Convert(
                        operatorFactory.Invoke(Expression.Convert(xParameter, integerType), Expression.Convert(yParameter, integerType)),
                        typeof(TEnum)),
                    xParameter,
                    yParameter);

            return binaryOrExpression.Compile();
        }
    }
}

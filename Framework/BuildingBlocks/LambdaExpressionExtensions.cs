using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Contains extenion methods for the <see cref="LambdaExpression" /> class.
    /// </summary>
    public static class LambdaExpressionExtensions
    {        
        internal static object Invoke<TMessage, TValue>(this Expression<Func<TMessage, TValue>> expression, TMessage message)
        {            
            var otherFactoryDelegate = expression.Compile();
            string memberName;

            if (expression.TryExtractMemberName(out memberName))
            {
                return string.Format("{0} ({1})", memberName, Invoke(otherFactoryDelegate, message));
            }
            return Invoke(otherFactoryDelegate, message);
        }

        private static object Invoke<TMessage, TValue>(Func<TMessage, TValue> otherFactory, TMessage message)
        {
            var value = otherFactory.Invoke(message) as object;
            if (value == null)
            {
                return StringTemplate.NullValue;
            }
            return value;
        }

        /// <summary>
        /// Returns the name of the field or property accessed by the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression to analyze.</param>
        /// <returns>The name of the field or property that is accessed.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The name of a field or property could not be retrieved from the specified <paramref name="expression"/>.
        /// </exception>
        public static string ExtractMemberName(this LambdaExpression expression)
        {
            string memberName;

            if (TryExtractMemberName(expression, out memberName))
            {
                return memberName;
            }
            throw NewExpressionNotSupportedException(expression);
        }

        /// <summary>
        /// Attempts to return the name of the field or property accessed by the specified <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression to analyze.</param>
        /// <param name="memberName">
        /// If this method returns <c>true</c>, this parameter will refer to the name of the member that was extracted from the expression;
        /// otherwise it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the name could be extracted; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expression"/> is <c>null</c>.
        /// </exception>
        public static bool TryExtractMemberName(this LambdaExpression expression, out string memberName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            MemberExpression memberExpression;

            if (TryCastToMemberExpression(expression, out memberExpression))
            {
                memberName = memberExpression.Member.Name;
                return true;
            }
            memberName = null;
            return false;
        }

        private static bool TryCastToMemberExpression(LambdaExpression lambdaExpression, out MemberExpression memberExpression)
        {
            var unaryExpression = lambdaExpression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = (MemberExpression) unaryExpression.Operand;
                return true;
            }
            return (memberExpression = lambdaExpression.Body as MemberExpression) != null;            
        }

        private static Exception NewExpressionNotSupportedException(Expression expression)
        {
            var messageFormat = ExceptionMessages.ExpressionExtensions_UnsupportedExpression;
            var message = string.Format(messageFormat, expression.NodeType);
            return new ArgumentException(message, "expression");
        }  
    }
}

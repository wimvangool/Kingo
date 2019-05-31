using System;
using System.Linq.Expressions;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Contains extenion methods for the <see cref="LambdaExpression" /> class.
    /// </summary>
    public static class LambdaExpressionExtensions
    {        
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
        public static Identifier ExtractMemberName(this LambdaExpression expression)
        {
            if (TryExtractMemberName(expression, out var memberName))
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
        public static bool TryExtractMemberName(this LambdaExpression expression, out Identifier memberName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }            
            var bodyExpression = expression.Body;

            var unaryExpression = bodyExpression as UnaryExpression;
            if (unaryExpression != null)
            {
                if (unaryExpression.NodeType == ExpressionType.ArrayLength)
                {
                    memberName = Identifier.Parse("Length");
                    return true;
                }
                bodyExpression = unaryExpression;
            }            
            if (TryCastToMemberExpression(bodyExpression, out var memberExpression))
            {
                memberName = Identifier.Parse(memberExpression.Member.Name);
                return true;
            }
            memberName = null;
            return false;
        }

        private static bool TryCastToMemberExpression(Expression expression, out MemberExpression memberExpression) =>
             (memberExpression = expression as MemberExpression) != null;

        private static Exception NewExpressionNotSupportedException(Expression expression)
        {
            var messageFormat = ExceptionMessages.ExpressionExtensions_UnsupportedExpression;
            var message = string.Format(messageFormat, expression.NodeType);
            return new ArgumentException(message, nameof(expression));
        }  
    }
}

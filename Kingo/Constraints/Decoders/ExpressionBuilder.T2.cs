using System;
using System.Linq.Expressions;
using Kingo.Resources;

namespace Kingo.Constraints.Decoders
{
    internal abstract class ExpressionBuilder : ExpressionVisitor
    {
        protected abstract LambdaExpression FieldOrPropertyExpression
        {
            get;
        }        

        protected Exception NewNotSupportedExpressionException(object node)
        {
            var messageFormat = ExceptionMessages.ExpressionBuilder_ExpressionNotSupported;
            var message = string.Format(messageFormat, node);
            return new ExpressionNotSupportedException(FieldOrPropertyExpression, message);
        }

        protected static bool IsSupported(ExpressionType nodeType)
        {
            return
                nodeType == ExpressionType.ArrayIndex ||
                nodeType == ExpressionType.ArrayLength ||
                nodeType == ExpressionType.Call ||
                nodeType == ExpressionType.MemberAccess ||
                nodeType == ExpressionType.Parameter;
        }
    }
}

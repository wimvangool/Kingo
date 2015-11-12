using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class LeftExpressionFactory : ExpressionVisitor
    {
        private readonly LambdaExpression _expression;
        private Expression _leftExpression;

        private LeftExpressionFactory(LambdaExpression expression)
        {
            _expression = expression;            
        }

        public override Expression Visit(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.MemberAccess:
                case ExpressionType.Parameter:
                    return base.Visit(node);
                default:
                    throw NewNotSupportedExpressionException(_expression);
            }            
        }



        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.NodeType == ExpressionType.Parameter)
            {
                _leftExpression = node;
            }
            return base.VisitMember(node);
        }        

        internal static LambdaExpression CreateLeftExpression(LambdaExpression expression)
        {
            var splitter = new LeftExpressionFactory(expression);

            splitter.Visit(expression.Body);

            //if (splitter._leftExpression ==)
            throw new NotImplementedException();
        }               

        private static Exception NewNotSupportedExpressionException(Expression expression)
        {
            var messageFormat = ExceptionMessages.MemberExpressionSplitter_ExpressionNotSupported;
            var message = string.Format(messageFormat, expression);
            return new ExpressionNotSupportedException(expression, message);
        }
    }
}

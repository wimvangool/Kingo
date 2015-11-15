using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class MemberExpressionPairNode
    {
        internal abstract LambdaExpression Expression
        {
            get;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }

        internal bool IsTrivialParameterExpression()
        {
            return IsTrivialParameterExpression(Expression);
        }

        internal static bool IsTrivialParameterExpression(LambdaExpression expression)
        {
            return expression.Body.NodeType == ExpressionType.Parameter;
        }        
    }
}

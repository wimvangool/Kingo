using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExpressionRightNode : MemberExpressionPairNode
    {
        private readonly LambdaExpression _expression;

        internal MemberExpressionRightNode(LambdaExpression expression)
        {
            _expression = expression;
        }

        internal override LambdaExpression Expression
        {
            get { return _expression; }
        }        
    }
}

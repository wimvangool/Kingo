using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExpressionLeftNode : MemberExpressionPairNode
    {
        private readonly LambdaExpression _expression;
        private readonly IReadOnlyList<Expression> _indexerArguments;

        internal MemberExpressionLeftNode(LambdaExpression expression, IReadOnlyList<Expression> indexerArguments)
        {
            _expression = expression;
            _indexerArguments = indexerArguments;
        }       

        internal override LambdaExpression Expression
        {
            get { return _expression; }
        }

        internal ParameterExpression Parameter
        {
            get { return _expression.Parameters[0]; }
        }

        internal bool IsIndexer
        {
            get { return IndexerArguments != null && IndexerArguments.Count > 0; }
        }

        internal bool IsMemberAccess
        {
            get { return !IsTrivialParameterExpression() && !IsIndexer; }
        }

        internal IReadOnlyList<Expression> IndexerArguments
        {
            get { return _indexerArguments; }
        }

        internal static bool IsTrivialParameterExpression(LambdaExpression expression, out MemberExpressionLeftNode left)
        {
            if (IsTrivialParameterExpression(expression))
            {
                left = new MemberExpressionLeftNode(expression, null);
                return true;
            }
            left = null;
            return false;
        }        
    }
}

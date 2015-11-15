using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExpressionLeftNode : MemberExpressionPairNode
    {
        private readonly LambdaExpression _expression;
        private readonly IReadOnlyList<Expression> _indexerArguments;
        private readonly bool _isArrayLength;

        internal MemberExpressionLeftNode(LambdaExpression expression, IReadOnlyList<Expression> indexerArguments, bool isArrayLength)
        {
            _expression = expression;
            _indexerArguments = indexerArguments;
            _isArrayLength = isArrayLength;
        }      

        internal override LambdaExpression Expression
        {
            get { return _expression; }
        }

        internal bool IsIndexer
        {
            get { return IndexerArguments != null && IndexerArguments.Count > 0; }
        }

        internal bool IsMemberAccess
        {
            get { return !IsTrivialParameterExpression() && !IsIndexer; }
        }

        internal bool IsArrayLength
        {
            get { return _isArrayLength; }
        }

        internal ParameterExpression Parameter
        {
            get { return _expression.Parameters[0]; }
        }        

        internal IReadOnlyList<Expression> IndexerArguments
        {
            get { return _indexerArguments; }
        }

        internal static bool IsTrivialParameterExpression(LambdaExpression expression, out MemberExpressionLeftNode left)
        {
            if (IsTrivialParameterExpression(expression))
            {
                left = new MemberExpressionLeftNode(expression, null, false);
                return true;
            }
            left = null;
            return false;
        }        
    }
}

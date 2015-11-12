﻿using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExpressionPair
    {
        internal readonly LambdaExpression LeftExpression;
        internal readonly LambdaExpression RightExpression;

        private MemberExpressionPair(LambdaExpression leftExpression, LambdaExpression rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }                

        internal static MemberExpressionPair SplitUp(LambdaExpression expression)
        {
            var leftExpression = LeftExpressionFactory.CreateLeftExpression(expression);            

            return new MemberExpressionPair(leftExpression, null);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberExpressionPair
    {
        #region [====== Splitter ======]

        private sealed class Splitter : ExpressionVisitor
        {
            private readonly LambdaExpression _expression;

            internal Splitter(LambdaExpression expression)
            {
                _expression = expression;
            }

            private bool IsDone
            {
                get { return LeftExpressionBody != null; }
            }

            internal Expression LeftExpressionBody
            {
                get;
                private set;
            }

            internal IReadOnlyList<Expression> IndexerArguments
            {
                get;
                private set;
            }

            internal ParameterExpression RightExpressionParameter
            {
                get;
                private set;
            }

            public override Expression Visit(Expression node)
            {
                if (IsDone || IsMemberOrIndexerExpression(node))
                {
                    return base.Visit(node);
                }
                throw NewNotSupportedExpressionException(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                // Check if expression is of the form 'm.Member'.
                if (node.Expression.NodeType == ExpressionType.Parameter)
                {
                    LeftExpressionBody = node;
                    
                    return ReplaceWithParameter(node.Type);
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                // Check if expression is of the form 'm[<arguments>]'.
                if (node.Left.NodeType == ExpressionType.Parameter)
                {
                    LeftExpressionBody = node.Left;
                    IndexerArguments = new [] { node.Right };

                    return ReplaceWithParameter(node.Type);
                }
                return base.VisitBinary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // Check if expression is of the form 'm[<arguments>]'.
                if (IsIndexerExpression(node))
                {
                    LeftExpressionBody = node.Object;
                    IndexerArguments = node.Arguments;

                    return ReplaceWithParameter(node.Type);
                }
                throw NewNotSupportedExpressionException(_expression);
            }

            private ParameterExpression ReplaceWithParameter(Type parameterType)
            {
                return RightExpressionParameter = Expression.Parameter(parameterType, _expression.Parameters[0].Name);
            }

            private static bool IsMemberOrIndexerExpression(Expression node)
            {
                return
                    node.NodeType == ExpressionType.ArrayIndex ||
                    node.NodeType == ExpressionType.Call ||
                    node.NodeType == ExpressionType.MemberAccess;
            }

            private static bool IsIndexerExpression(MethodCallExpression expression)
            {
                PropertyInfo indexer;

                if (IsIndexerCandidate(expression) && TryGetIndexer(expression.Object, expression.Arguments, out indexer))
                {
                    // Here we try to match, e.g., 'get_Item' with 'Item', to ensure
                    // the specified method is actually an indexer instead of a regular method
                    // with the same internal name of an indexer.
                    return expression.Method.Name.Substring(4) == indexer.Name;
                }
                return false;
            }           

            private static bool IsIndexerCandidate(MethodCallExpression expression)
            {
                return
                    expression.Object != null &&
                    expression.Object.NodeType == ExpressionType.Parameter &&
                    expression.Method.MemberType == MemberTypes.Method &&
                    expression.Arguments.Count > 0 &&
                    expression.Method.Name.StartsWith("get_"); 
            }

            private static bool TryGetIndexer(Expression instance, IEnumerable<Expression> arguments, out PropertyInfo indexer)
            {
                var instanceType = instance.Type;
                var argumentTypes = arguments.Select(argument => argument.Type);

                return CollectionConstraints.TryGetIndexer(instanceType, argumentTypes, out indexer);
            }
        }

        #endregion

        internal readonly MemberExpressionLeftNode Left;
        internal readonly MemberExpressionRightNode Right;

        private MemberExpressionPair(MemberExpressionLeftNode left,  MemberExpressionRightNode right)
        {
            Left = left;
            Right = right;
        }                

        internal static MemberExpressionPair SplitUp(LambdaExpression expression)
        {
            MemberExpressionLeftNode left;
            MemberExpressionRightNode right;

            if (MemberExpressionLeftNode.IsTrivialParameterExpression(expression, out left))
            {
                return new MemberExpressionPair(left, null);
            }
            var splitter = new Splitter(expression);
            var rightExpressionBody = splitter.Visit(expression.Body);
            var leftExpressionBody = splitter.LeftExpressionBody;

            var leftExpression = CreateLambdaExpression(leftExpressionBody, expression.Parameters[0]);
            var rightExpression = CreateLambdaExpression(rightExpressionBody, splitter.RightExpressionParameter);

            left = new MemberExpressionLeftNode(leftExpression, splitter.IndexerArguments);
            right = new MemberExpressionRightNode(rightExpression);

            return new MemberExpressionPair(left, right);            
        }  
      
        private static LambdaExpression CreateLambdaExpression(Expression expressionBody, ParameterExpression parameter)
        {
            return Expression.Lambda(expressionBody, parameter);
        }

        private static Exception NewNotSupportedExpressionException(Expression expression)
        {
            var messageFormat = ExceptionMessages.MemberExpressionPair_ExpressionNotSupported;
            var message = string.Format(messageFormat, expression);
            return new ExpressionNotSupportedException(expression, message);
        }
    }
}

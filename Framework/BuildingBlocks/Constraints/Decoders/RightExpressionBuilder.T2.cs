using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints.Decoders
{
    internal sealed class RightExpressionBuilder<T, TValue> : ExpressionBuilder
    {
        private readonly MemberExpressionDecoder<T, TValue> _interpreter;
        private readonly LambdaExpression _leftExpression;
        private readonly Stack<Expression> _rightExpressionSource;
        private readonly Stack<Expression> _rightExpressionDestination;
        private readonly MethodCallDecorator<T, TValue> _methodCallDecorator;
        private ParameterExpression _valueParameter;

        internal RightExpressionBuilder(MemberExpressionDecoder<T, TValue> interpreter, LambdaExpression leftExpression, Stack<Expression> rightExpressionSource, MethodCallDecorator<T, TValue> methodCallDecorator)
        {
            _interpreter = interpreter;
            _leftExpression = leftExpression;
            _rightExpressionSource = rightExpressionSource;
            _rightExpressionDestination = new Stack<Expression>(rightExpressionSource.Count);
            _methodCallDecorator = methodCallDecorator;
        }

        protected override LambdaExpression FieldOrPropertyExpression
        {
            get { return _interpreter.FieldOrPropertyExpression; }
        }

        internal MethodCallExpressionBuilder<T, TValue> BuildRightExpression()
        {
            // Step 1: Rebuild Right Expression by copying and assembling expression on the destination stack.
            //         This will leave the destination stack with 0 or 1 expression(s).
            while (_rightExpressionSource.Count > 0)
            {
                Visit(_rightExpressionSource.Pop());
            }

            // Step 2: Finalize Right Expression by creating a LambdaExpression of it (assuming it exists)
            //         and finalize the methodCallDecorator if required.
            var methodCallDecorator = _methodCallDecorator;
            var rightExpression = CreateRightExpression();
            if (rightExpression != null)
            {
                methodCallDecorator = methodCallDecorator.Append(new AndCallAppender<T, TValue>(rightExpression));
            }            
            return new MethodCallExpressionBuilder<T, TValue>(_interpreter, _leftExpression, methodCallDecorator);
        }

        private LambdaExpression CreateRightExpression()
        {
            if (_rightExpressionDestination.Count > 0)
            {
                var rightExpression = _rightExpressionDestination.Pop();
                if (rightExpression.NodeType != ExpressionType.Parameter)
                {
                    return Expression.Lambda(rightExpression, _interpreter.InstanceParameter, _valueParameter);
                }
            }
            return null;
        }

        public override Expression Visit(Expression node)
        {
            if (IsSupported(node.NodeType))
            {
                return base.Visit(node);
            }
            throw NewNotSupportedExpressionException(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            // NB: node.NodeType == ExpressionType.ArrayIndex.
            var leftOperand = _rightExpressionDestination.Pop();

            _rightExpressionDestination.Push(Expression.MakeBinary(
                node.NodeType,
                leftOperand,
                node.Right
            ));
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // NB: node.NodeType == ExpressionType.MemberAccess.
            var leftOperand = _rightExpressionDestination.Pop();

            _rightExpressionDestination.Push(Expression.MakeMemberAccess(
                leftOperand,
                node.Member
            ));
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // NB: node.NodeType == ExpressionType.Call.
            var leftOperand = _rightExpressionDestination.Pop();

            _rightExpressionDestination.Push(Expression.Call(
                leftOperand,
                node.Method,
                node.Arguments
            ));
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            // NB: node.NodeType == ExpressionType.ArrayLength.
            var leftOperand = _rightExpressionDestination.Pop();

            _rightExpressionDestination.Push(Expression.MakeUnary(
                node.NodeType,
                leftOperand,
                node.Type
            ));
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // NB: node.NodeType == ExpressionType.Parameter.
            _rightExpressionDestination.Push(node);
            _valueParameter = node;
            return node;
        }
    }
}

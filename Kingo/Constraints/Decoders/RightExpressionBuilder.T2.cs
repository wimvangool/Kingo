using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.Constraints.Decoders
{
    internal sealed class RightExpressionBuilder<T, TValue> : ExpressionBuilder
    {
        private readonly MemberExpressionDecoder<T, TValue> _interpreter;
        private readonly LambdaExpression _leftExpression;
        private readonly Stack<Expression> _rightExpressionSource;
        private readonly Stack<Expression> _rightExpressionDestination;
        private MethodCallDecorator<T, TValue> _methodCallDecorator;
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
            //         and concatenate .IsNotNull().And(<right_expression>) if required.            
            var rightExpression = CreateRightExpression();
            if (rightExpression != null)
            {
                _methodCallDecorator += new IsNotNullAndCallAppender<T, TValue>(rightExpression);
            }            
            return new MethodCallExpressionBuilder<T, TValue>(_interpreter, _leftExpression, _methodCallDecorator);
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

            // When the MemberExpression is of the form 'x.Value', where x
            // is a Nullable<> and Value is thus the Value-property accessor,
            // we replace the MemberExpression with a checked call to .HasValue()
            // in the method call chain.
            // On the stack, we replace the parameter with a parameter of the same
            // name but with the type that was nulled by the nullable to represent
            // the result of the .Value-expression.
            if (leftOperand.NodeType == ExpressionType.Parameter && IsNullableValue(node))
            {
                _methodCallDecorator += new HasValueCallAppender<T, TValue>();
                _rightExpressionDestination.Push(_valueParameter = GetValueOf(_valueParameter));

                return node;
            }
            _rightExpressionDestination.Push(Expression.MakeMemberAccess(
                leftOperand,
                node.Member
            ));
            return node;
        }        

        private static bool IsNullableValue(MemberExpression node)
        {
            return
                node.Member.Name == "Value" &&
                node.Expression.Type.IsValueType &&
                node.Expression.Type.IsGenericType &&
                node.Expression.Type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static ParameterExpression GetValueOf(ParameterExpression parameter)
        {
            var valueType = parameter.Type.GetGenericArguments()[0];
            var name = parameter.Name;

            return Expression.Parameter(valueType, name);
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

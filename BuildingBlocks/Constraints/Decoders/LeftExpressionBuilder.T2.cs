using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.BuildingBlocks.Constraints.Decoders
{
    internal sealed class LeftExpressionBuilder<T, TValue> : ExpressionBuilder
    {
        private readonly MemberExpressionDecoder<T, TValue> _interpreter;
        private readonly Stack<Expression> _rightExpressionSource; 

        private LambdaExpression _leftExpression;
        private MethodCallDecorator<T, TValue> _methodCallDecorator;               

        internal LeftExpressionBuilder(MemberExpressionDecoder<T, TValue> interpreter)
        {
            _interpreter = interpreter;
            _rightExpressionSource = new Stack<Expression>();
            _methodCallDecorator = new NullAppender<T, TValue>();
        }

        protected override LambdaExpression FieldOrPropertyExpression
        {
            get { return _interpreter.FieldOrPropertyExpression; }
        }        

        private bool IsBuildingLeftExpression
        {
            get { return _leftExpression == null; }
        }

        internal RightExpressionBuilder<T, TValue> BuildLeftExpression()
        {
            Visit(_interpreter.FieldOrPropertyExpression.Body);

            return new RightExpressionBuilder<T, TValue>(_interpreter, _leftExpression, _rightExpressionSource, _methodCallDecorator);
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                return null;
            }
            if (IsSupported(node.NodeType) || !IsBuildingLeftExpression)
            {
                return base.Visit(node);
            }
            throw NewNotSupportedExpressionException(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            // NB: node.NodeType == ExpressionType.ArrayIndex.
            if (IsBuildingLeftExpression)
            {
                // Check if expression is of the form 'm[<arguments>]' (array index).
                if (MatchesPrimaryParameter(node.Left))
                {
                    // <expression>.HasItem<TValue>(<index>)                    
                    _leftExpression = _interpreter.CreateLeftExpression(node.Left);
                    _methodCallDecorator += new HasItemCallAppender<T, TValue>(_interpreter.InstanceParameter, new[] { node.Right });
                    _rightExpressionSource.Push(CreateValueParameter(node.Type));

                    return node;
                }
                _rightExpressionSource.Push(node);
            }
            return base.VisitBinary(node);
        }        

        protected override Expression VisitMember(MemberExpression node)
        {
            // NB: node.NodeType == ExpressionType.MemberAccess.
            if (IsBuildingLeftExpression)
            {
                // Check if expression is of the form 'm.Member' (field or property access).
                if (MatchesPrimaryParameter(node.Expression))
                {                    
                    _leftExpression = _interpreter.CreateLeftExpression(node);
                    _rightExpressionSource.Push(CreateValueParameter(node.Type));

                    return node;
                }                
                _rightExpressionSource.Push(node);
            }
            return base.VisitMember(node);
        }               

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // NB: node.NodeType == ExpressionType.Call.
            if (IsBuildingLeftExpression)
            {
                if (IsIndexerExpression(node))
                {
                    // Check if expression is of the form 'm[<arguments>]' (indexer).
                    if (MatchesPrimaryParameter(node.Object))
                    {
                        // <expression>.HasItem<TValue>(<arguments>)                        
                        _leftExpression = _interpreter.CreateLeftExpression(node.Object);
                        _methodCallDecorator += new HasItemCallAppender<T, TValue>(_interpreter.InstanceParameter, node.Arguments);
                        _rightExpressionSource.Push(CreateValueParameter(node.Type));

                        return node;
                    }
                    _rightExpressionSource.Push(node);

                    return base.VisitMethodCall(node);
                }
                throw NewNotSupportedExpressionException(node);
            }
            return base.VisitMethodCall(node);
        }        

        protected override Expression VisitUnary(UnaryExpression node)
        {
            // NB: node.NodeType == ExpressionType.ArrayLength.
            if (IsBuildingLeftExpression)
            {
                // Check if expression is of the form 'm.Length'.
                if (MatchesPrimaryParameter(node.Operand))
                {
                    // <expression>.Length()                    
                    _leftExpression = _interpreter.CreateLeftExpression(node.Operand);
                    _methodCallDecorator += new LengthCallAppender<T, TValue>();
                    _rightExpressionSource.Push(CreateValueParameter(node.Type));

                    return node;
                }
                _rightExpressionSource.Push(node);
            }
            return base.VisitUnary(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // NB: node.NodeType == ExpressionType.Parameter.
            if (IsBuildingLeftExpression)
            {
                _leftExpression = FieldOrPropertyExpression;

                return node;
            }
            return base.VisitParameter(node);
        }        

        private bool MatchesPrimaryParameter(Expression childExpression)
        {
            return ReferenceEquals(_interpreter.PrimaryParameter, childExpression);
        }

        private static bool IsIndexerExpression(MethodCallExpression expression)
        {
            if (expression.Object != null)
            {
                // First check if we are dealing with an index-operation on a multi-dimensional array.
                if (expression.Object.Type.IsArray)
                {
                    return IsMultiDimensionalArrayIndexer(expression);
                }
                PropertyInfo indexer;

                if (IsIndexerCandidate(expression) && TryGetIndexer(expression.Object, expression.Arguments, out indexer))
                {
                    // Here we try to match, e.g., 'get_Item' with 'Item', to ensure
                    // the specified method is actually an indexer instead of a regular method
                    // with the same internal name of an indexer.
                    return expression.Method.Name.Substring(4) == indexer.Name;
                }
            }
            return false;
        }

        private static bool IsMultiDimensionalArrayIndexer(MethodCallExpression expression)
        {
            return
                expression.Method.Name == "Get" &&
                expression.Arguments.Count > 1;
        }

        private static bool IsIndexerCandidate(MethodCallExpression expression)
        {
            return                
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

        private static ParameterExpression CreateValueParameter(Type parameterType)
        {
            return Expression.Parameter(parameterType, CreateValueParameterName());
        }

        private static string CreateValueParameterName()
        {
            return string.Format("x_{0:N}", Guid.NewGuid());
        }
    }
}

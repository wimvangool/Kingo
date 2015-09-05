using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    internal sealed class DisplayFormatFactory : ExpressionVisitor
    {
        private readonly Type _parameterType;
        private readonly string _oldParameterName;
        private readonly string _newParameterName;

        private DisplayFormatFactory(Type parameterType, string oldParameterName, string newParameterName)
        {
            _parameterType = parameterType;
            _oldParameterName = oldParameterName;
            _newParameterName = newParameterName;
        }

        private Expression RenameParameterOf(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Name == _oldParameterName)
            {
                return Expression.Parameter(_parameterType, _newParameterName);
            }
            return node;
        }

        internal static StringTemplate InferDisplayFormat<TValue>(Expression<Func<TValue, bool>> constraint)
        {
            var oldParameterName = constraint.Parameters[0].Name;
            var newParameterName = "p" + Guid.NewGuid().ToString("N");

            var visitor = new DisplayFormatFactory(typeof(TValue), oldParameterName, newParameterName);
            var expression = (LambdaExpression) visitor.RenameParameterOf(constraint);

            var displayFormat = expression.Body.ToString()
                .Replace("{", "{{")
                .Replace("}", "}}")
                .Replace(newParameterName, @"{member.Name}");

            return StringTemplate.Parse(displayFormat);
        }        
    }
}

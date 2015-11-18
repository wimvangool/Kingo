using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints.Decoders
{
    internal abstract class MethodCallDecorator<T, TValue>
    {        
        public abstract MethodCallExpression Decorate(MethodCallExpression expression);

        public virtual MethodCallDecorator<T, TValue> Append(MethodCallDecorator<T, TValue> appender)
        {
            return new CompositeAppender<T, TValue>(this, appender);
        }

        public override string ToString()
        {
            return string.Join(".", MethodCalls());
        }

        internal abstract IEnumerable<string> MethodCalls();

        protected static Type GetGenericArgumentType(Type builderType, int index)
        {
            return builderType.GetGenericArguments()[index];
        }        

        protected static Expression NoErrorMessage()
        {
            return Expression.Constant(null, typeof(string));
        } 
    }
}

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Kingo.DynamicMethods
{
    internal sealed class GetHashCodeMethod<TValue> : GetHashCodeMethod
    {
        #region [====== Instance Members ======]

        private readonly Func<TValue, int> _implementation;
        private readonly string _representation;

        private GetHashCodeMethod(Expression<Func<TValue, int>> implementation)
        {
            _implementation = implementation.Compile();
            _representation = implementation.Body.ToString();
        }

        internal override int Execute(object instance) => Execute((TValue) instance);

        private int Execute(TValue instance) => _implementation.Invoke(instance);

        public override string ToString() => _representation;

        #endregion

        #region [====== Static Members ======]

        [UsedImplicitly]
        public static GetHashCodeMethod<TValue> FromExpression(Expression expression, ParameterExpression instance) => new GetHashCodeMethod<TValue>(Expression.Lambda<Func<TValue, int>>(expression, instance));

        #endregion
    }
}

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Kingo.DynamicMethods
{
    internal sealed class EqualsMethod<TValue> : EqualsMethod
    {
        #region [====== Instance Members ======]

        private readonly Func<TValue, TValue, bool> _implementation;
        private readonly string _representation;

        private EqualsMethod(Expression<Func<TValue, TValue, bool>> implementation)
        {
            _implementation = implementation.Compile();
            _representation = implementation.Body.ToString();
        }

        internal override bool Execute(object left, object right) => Execute((TValue) left, (TValue) right);

        private bool Execute(TValue left, TValue right) => _implementation.Invoke(left, right);

        public override string ToString() => _representation;

        #endregion

        #region [====== Static Members ======]

        [UsedImplicitly]
        public static EqualsMethod<TValue> FromExpression(Expression equalsExpression, ParameterExpression left, ParameterExpression right) => new EqualsMethod<TValue>(Expression.Lambda<Func<TValue, TValue, bool>>(equalsExpression, left, right));

        #endregion
    }
}

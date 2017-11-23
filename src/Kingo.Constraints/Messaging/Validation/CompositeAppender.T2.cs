﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    internal sealed class CompositeAppender<T, TValue> : MethodCallDecorator<T, TValue>
    {
        private readonly MethodCallDecorator<T, TValue> _leftAppender;
        private readonly MethodCallDecorator<T, TValue> _rightAppender;

        internal CompositeAppender(MethodCallDecorator<T, TValue> leftAppender, MethodCallDecorator<T, TValue> rightAppender)
        {
            _leftAppender = leftAppender;
            _rightAppender = rightAppender;
        }

        internal override IEnumerable<string> MethodCalls() => _leftAppender.MethodCalls().Concat(_rightAppender.MethodCalls());

        public override MethodCallExpression Decorate(MethodCallExpression expression) => _rightAppender.Decorate(_leftAppender.Decorate(expression));
    }
}

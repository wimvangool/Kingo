using System;
using System.Collections.Generic;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing.TestEngine
{
    internal class Operand<T> : IOperand<T>
    {
        private readonly IScenario _scenario;
        private readonly T _expression;

        public Operand(IScenario scenario, T expression)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException("scenario");
            }
            _scenario = scenario;
            _expression = expression;
        }

        #region [====== IOperand Members ======]

        public IScenario Scenario
        {
            get { return _scenario; }
        }

        public T Value
        {
            get { return _expression; }
        }

        public IConjunctionStatement<TExpected> IsA<TExpected>()
        {
            Type type = typeof(TExpected);
            Type typeOfExpression = GetTypeOfExpression();

            if (type.IsAssignableFrom(typeOfExpression))
            {
                return new ConjunctionStatement<TExpected>((TExpected)(_expression as object), true);
            }
            _scenario.Fail(FailureMessages.ExpressionNotOfCompatibleType, type, typeOfExpression);

            return new ConjunctionStatement<TExpected>(default(TExpected), false);
        }

        public void IsA(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            Type typeOfExpression = GetTypeOfExpression();

            if (!type.IsAssignableFrom(typeOfExpression))
            {
                _scenario.Fail(FailureMessages.ExpressionNotOfCompatibleType, type, typeOfExpression); 
            }            
        }

        public void IsNotA<TUnexpected>()
        {
            IsNotA(typeof(TUnexpected));
        }

        public void IsNotA(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            Type typeOfExpression = GetTypeOfExpression();

            if (type.IsAssignableFrom(typeOfExpression))
            {
                _scenario.Fail(FailureMessages.ExpressionOfCompatibleType, type, typeOfExpression);
            }            
        }
     
        private Type GetTypeOfExpression()
        {
            return ReferenceEquals(_expression, null) ? typeof(T) : _expression.GetType();
        }

        public void IsNull()
        {
            if (ReferenceEquals(_expression, null))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionIsNotNull, _expression);
        }

        public void IsNotNull()
        {
            if (!ReferenceEquals(_expression, null))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionIsNull);
        }

        public void IsSameInstanceAs(T expression)
        {
            if (ReferenceEquals(_expression, expression))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionIsNotSameTheInstance, expression, _expression);
        }

        public void IsNotSameInstanceAs(T expression)
        {
            if (!ReferenceEquals(_expression, expression))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionIsTheSameInstance, _expression);
        }           

        public void IsEqualTo(T expression)
        {
            IsEqualTo(expression, null);
        }

        public void IsEqualTo(T expression, IEqualityComparer<T> comparer)
        {            
            if ((comparer ?? EqualityComparer<T>.Default).Equals(_expression, expression))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionNotEqual, expression, _expression);
        }

        public void IsNotEqualTo(T expression)
        {
            IsNotEqualTo(expression, null);
        }

        public void IsNotEqualTo(T expression, IEqualityComparer<T> comparer)
        {
            if (!(comparer ?? EqualityComparer<T>.Default).Equals(_expression, expression))
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionEqual, _expression);
        }

        public void IsSmallerThan(T expression, IComparer<T> comparer)
        {
            if ((comparer ?? Comparer<T>.Default).Compare(_expression, expression) < 0)
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionNotSmaller, expression, _expression);
        }

        public void IsSmallerThanOrEqualTo(T expression, IComparer<T> comparer)
        {
            if ((comparer ?? Comparer<T>.Default).Compare(_expression, expression) <= 0)
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionNotSmallerOrEqual, expression, _expression);
        }

        public void IsGreaterThan(T expression, IComparer<T> comparer)
        {
            if ((comparer ?? Comparer<T>.Default).Compare(_expression, expression) > 0)
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionNotGreater, expression, _expression);
        }

        public void IsGreaterThanOrEqualTo(T expression, IComparer<T> comparer)
        {
            if ((comparer ?? Comparer<T>.Default).Compare(_expression, expression) >= 0)
            {
                return;
            }
            _scenario.Fail(FailureMessages.ExpressionNotGreaterOrEqual, expression, _expression);
        }

        #endregion        
    }
}

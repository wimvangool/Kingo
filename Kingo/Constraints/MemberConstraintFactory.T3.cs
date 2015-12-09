using System;
using Kingo.Messaging;

namespace Kingo.Constraints
{
    internal sealed class MemberConstraintFactory<T, TValueIn, TValueOut> : IErrorMessageWriter<T>
    {
        private readonly MemberFactory<T, TValueIn> _memberFactory;               
        private readonly Func<T, IMemberConstraint<TValueIn, TValueOut>> _memberConstraintFactory;        

        internal MemberConstraintFactory(MemberFactory<T, TValueIn> memberFactory, Func<T, IFilter<TValueIn, TValueOut>> constraintFactory)            
        {
            _memberFactory = memberFactory;            
            _memberConstraintFactory = instance => new MemberConstraint<TValueIn, TValueOut>(constraintFactory.Invoke(instance));
        }

        private MemberConstraintFactory(MemberFactory<T, TValueIn> memberFactory, Func<T, IMemberConstraint<TValueIn, TValueOut>> memberConstraintFactory)
        {
            _memberFactory = memberFactory;           
            _memberConstraintFactory = memberConstraintFactory;            
        }        

        internal MemberFactory<T, TValueOut> CreateChildMember()
        {
            return _memberFactory.CreateChildMember(_memberConstraintFactory);            
        }

        internal MemberConstraintFactory<T, TValueIn, TResult> And<TResult>(Func<T, Tuple<IFilter<TValueOut, TResult>, MemberTransformer>> memberConstraintFactory)
        {                      
            return new MemberConstraintFactory<T, TValueIn, TResult>(_memberFactory, instance =>
            {
                var components = memberConstraintFactory.Invoke(instance);                
                var left = _memberConstraintFactory.Invoke(instance);
                var right = new MemberConstraint<TValueOut, TResult>(components.Item1, components.Item2);

                return left.And(right);
            });
        }        

        #region [====== ErrorMessageWriter ======]

        public bool WriteErrorMessages(T instance, IErrorMessageReader reader)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }            
            var constraint = CreateMemberConstraint(instance);
            var member = _memberFactory.CreateMember(instance);
            Member<TValueOut> transformedMember;

            return constraint.IsNotSatisfiedBy(member, reader, out transformedMember);
        }

        private IMemberConstraint<TValueIn, TValueOut> CreateMemberConstraint(T instance)
        {
            return _memberConstraintFactory.Invoke(instance);
        }

        #endregion
    }
}

using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberConstraintFactory<T, TValueIn, TValueOut> : IErrorMessageWriter<T>
    {
        private readonly Member<T, TValueIn> _originalMember;
        private readonly MemberByTransformation _transformedMember;
        private readonly IMemberTransformation _transformation;
        private readonly Func<T, IConstraint<TValueIn, TValueOut>> _constraintFactory;        

        internal MemberConstraintFactory(Member<T, TValueIn> originalMember, Func<T, IConstraint<TValueIn, TValueOut>> constraintFactory)
            : this(originalMember, constraintFactory, originalMember.EnableTransformation(), new MemberNameTransformation(null)) { }

        private MemberConstraintFactory(Member<T, TValueIn> originalMember, Func<T, IConstraint<TValueIn, TValueOut>> constraintFactory, MemberByTransformation transformedMember, IMemberTransformation transformation)
        {
            _originalMember = originalMember;
            _transformedMember = transformedMember;
            _transformation = transformation;
            _constraintFactory = constraintFactory;            
        }

        internal Member Member
        {
            get { return _transformedMember; }
        }

        internal Member<T, TValueOut> CreateChildMember()
        {
            return _originalMember.CreateChildMember(_constraintFactory);
        }        

        internal MemberConstraintFactory<T, TValueIn, TResult> And<TResult>(Func<T, IConstraint<TValueOut, TResult>> constraintFactory, IMemberTransformation nextTransformation)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            var transformedMember = _transformation.Execute(_transformedMember, typeof(TValueOut));

            return new MemberConstraintFactory<T, TValueIn, TResult>(_originalMember, message =>
            {
                var left = _constraintFactory.Invoke(message);
                var right = constraintFactory.Invoke(message);

                return left.And(right);
            }, transformedMember, nextTransformation);
        }

        public bool WriteErrorMessages(T message, IErrorMessageReader reader)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }            
            var constraint = CreateConstraint(message);
            var value = _originalMember.GetValue(message);
            IErrorMessage errorMessage;

            if (constraint.IsNotSatisfiedBy(value, out errorMessage))
            {                
                _transformedMember.WriteErrorMessageTo(reader, errorMessage);
                return true;
            }
            return false;
        }

        private MemberConstraint<TValueIn, TValueOut> CreateConstraint(T message)
        {
            return new MemberConstraint<TValueIn, TValueOut>(_transformedMember, _constraintFactory.Invoke(message));
        }        
    }
}

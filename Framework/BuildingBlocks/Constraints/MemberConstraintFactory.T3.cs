using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberConstraintFactory<TMessage, TValueIn, TValueOut> : IErrorMessageWriter<TMessage>
    {
        private readonly Member<TMessage, TValueIn> _originalMember;
        private readonly MemberByTransformation _transformedMember;
        private readonly Func<TMessage, IConstraint<TValueIn, TValueOut>> _constraintFactory;
        private readonly Func<string, string> _nameSelector;

        internal MemberConstraintFactory(Member<TMessage, TValueIn> originalMember, Func<TMessage, IConstraint<TValueIn, TValueOut>> constraintFactory)
            : this(originalMember, constraintFactory, originalMember.EnableTransformation(), null) { }

        private MemberConstraintFactory(Member<TMessage, TValueIn> originalMember, Func<TMessage, IConstraint<TValueIn, TValueOut>> constraintFactory, MemberByTransformation transformedMember, Func<string, string> nameSelector)
        {
            _originalMember = originalMember;
            _transformedMember = transformedMember;
            _constraintFactory = constraintFactory;
            _nameSelector = nameSelector;
        }

        internal Member Member
        {
            get { return _transformedMember; }
        }

        internal Member<TMessage, TValueOut> CreateChildMember()
        {
            return _originalMember.CreateChildMember(_constraintFactory);
        }

        internal MemberConstraintFactory<TMessage, TValueIn, TResult> And<TResult>(Func<TMessage, IConstraint<TValueOut, TResult>> constraintFactory, Func<string, string> nameSelector)
        {
            if (constraintFactory == null)
            {
                throw new ArgumentNullException("constraintFactory");
            }
            var transformedMember = _transformedMember.Transform(typeof(TValueOut), _nameSelector);            

            return new MemberConstraintFactory<TMessage, TValueIn, TResult>(_originalMember, message =>
            {
                var left = _constraintFactory.Invoke(message);
                var right = constraintFactory.Invoke(message);

                return left.And(right);
            }, transformedMember, nameSelector);
        }

        public bool WriteErrorMessages(TMessage message, IErrorMessageReader reader)
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
                reader.Add(_transformedMember.FullName, errorMessage);
                return true;
            }
            return false;
        }

        private MemberConstraint<TValueIn, TValueOut> CreateConstraint(TMessage message)
        {
            return new MemberConstraint<TValueIn, TValueOut>(_transformedMember, _constraintFactory.Invoke(message));
        }        
    }
}

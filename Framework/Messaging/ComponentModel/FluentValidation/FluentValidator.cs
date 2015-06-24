using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> in which validation-errors are reported
    /// through a <see cref="ValidationErrorTreeBuilder" />.
    /// </summary>    
    public sealed class FluentValidator : IMessageValidator, IFluentValidator
    {        
        private readonly MemberSet _memberSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidator" /> class.
        /// </summary>        
        public FluentValidator()
        {                      
            _memberSet = new MemberSet();
        }

        /// <inheritdoc />
        public Member<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression)
        {
            return _memberSet.StartToAddConstraintsFor(memberExpression);
        }

        /// <inheritdoc /> 
        public Member<TValue> VerifyThat<TValue>(Func<TValue> valueFactory, string name)
        {
            return _memberSet.StartToAddConstraintsFor(valueFactory, name);
        }

        /// <inheritdoc />
        public Member<TValue> VerifyThat<TValue>(TValue value, string name)
        {
            return _memberSet.StartToAddConstraintsFor(value, name);
        }

        /// <inheritdoc />
        public ValidationErrorTree Validate()
        {
            var errorTreeBuilder = new ValidationErrorTreeBuilder();

            _memberSet.AddErrorMessagesTo(errorTreeBuilder);

            return errorTreeBuilder.BuildErrorTree();
        }        
    }
}

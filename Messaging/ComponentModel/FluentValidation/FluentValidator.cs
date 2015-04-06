using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> in which validation-errors are reported
    /// through a <see cref="ValidationErrorTreeBuilder" />.
    /// </summary>    
    public sealed class FluentValidator : IMessageValidator
    {        
        private readonly MemberSet _memberSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidator" /> class.
        /// </summary>        
        public FluentValidator()
        {                      
            _memberSet = new MemberSet();
        }

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        public Member<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression)
        {
            return _memberSet.StartToAddConstraintsFor(memberExpression);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="valueFactory">
        /// A delegate that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="name">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFactory"/> or <paramref name="name"/> is <c>null</c>.
        /// </exception> 
        public Member<TValue> VerifyThat<TValue>(Func<TValue> valueFactory, string name)
        {
            return _memberSet.StartToAddConstraintsFor(valueFactory, name);
        }

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="value">The value to add constraints for.</param>
        /// <param name="name">The name of the member to add constraints for.</param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception> 
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

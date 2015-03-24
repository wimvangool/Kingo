using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents a <see cref="IMessageValidator" /> in which validation-errors are reported
    /// through a <see cref="ValidationErrorTreeBuilder" />.
    /// </summary>    
    public sealed class FluentValidator : IMessageValidator, IMemberParent
    {        
        private readonly Dictionary<string, List<IErrorMessageProducer>> _memberConstraints;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidator" /> class.
        /// </summary>        
        public FluentValidator()
        {           
            _memberConstraints = new Dictionary<string, List<IErrorMessageProducer>>();
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
        public Member<TValue> VerifyThat<TValue>(Expression<Func<TValue>> memberExpression)
        {
            var member = new Member<TValue>(this, memberExpression);

            Add(member);

            return member;
        }

        /// <summary>
        /// Creates and returns a new <see cref="NullableMember{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="memberExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="NullableMember{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        public NullableMember<TValue> VerifyThat<TValue>(Expression<Func<TValue?>> memberExpression) where TValue : struct
        {
            var member = new NullableMember<TValue>(this, memberExpression);

            Add(member);

            return member;
        }

        private void Add(IMember member)
        {
            var memberName = member.Name;
            List<IErrorMessageProducer> producers;

            if (!_memberConstraints.TryGetValue(memberName, out producers))
            {
                _memberConstraints.Add(memberName, producers = new List<IErrorMessageProducer>());
            }
            producers.Add(member);            
        }

        void IMemberParent.ReplaceMember(string memberName, IErrorMessageProducer oldProducer, IErrorMessageProducer newProducer)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            List<IErrorMessageProducer> producers;

            if (_memberConstraints.TryGetValue(memberName, out producers) && producers.Remove(oldProducer))
            {
                producers.Add(newProducer);
            }
        }

        /// <inheritdoc />
        public ValidationErrorTree Validate()
        {
            var builder = new ValidationErrorTreeBuilder();

            foreach (var constraints in _memberConstraints)
            {
                var consumer = new ValidationErrorConsumer(builder, constraints.Key);

                foreach (var producer in constraints.Value)
                {
                    if (producer.Accept(consumer) > 0)
                    {
                        break;
                    }
                }
            }
            return builder.BuildErrorTree();
        }        
    }
}

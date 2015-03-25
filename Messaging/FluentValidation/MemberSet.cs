using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Linq.Expressions;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents a set of <see cref="IMember"/> instances that are validated by adding run-time constraints.
    /// </summary>
    public class MemberSet : IErrorMessageProducer, IEnumerable<IMember>
    {
        private readonly IErrorMessageConsumer _consumer;        
        private readonly Dictionary<string, IMember> _members;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSet" /> class.
        /// </summary>
        /// <param name="consumer">
        /// An optional <see cref="IErrorMessageConsumer"/> that can be used to collect errors as
        /// new constraints are being added to certain members.
        /// </param>
        public MemberSet(IErrorMessageConsumer consumer = null)
        {            
            _consumer = consumer;            
            _members = new Dictionary<string, IMember>();
        }

        /// <summary>
        /// The <see cref="IErrorMessageConsumer" /> that was specified to collect errors as
        /// new constraints are being added to certain members.
        /// </summary>
        public IErrorMessageConsumer Consumer
        {
            get { return _consumer; }
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
        public Member<TValue> Add<TValue>(Expression<Func<TValue>> memberExpression)
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
        /// <exception cref="ArgumentException">
        /// <paramref name="memberExpression"/> refers to a member that was already added.
        /// </exception>
        public NullableMember<TValue> Add<TValue>(Expression<Func<TValue?>> memberExpression) where TValue : struct
        {
            var member = new NullableMember<TValue>(this, memberExpression);

            Add(member);

            return member;
        }

        /// <summary>
        /// Replaces <paramref name="oldMember"/> by the specified <paramref name="newMember"/>.
        /// </summary>
        /// <param name="oldMember">The member to remove.</param>
        /// <param name="newMember">The member to add.</param>
        protected internal void Replace(IMember oldMember, IMember newMember)
        {
            if (Remove(oldMember))
            {
                Add(newMember);
            }
        }

        /// <summary>
        /// Removes the specified <paramref name="member"/> from the set.
        /// </summary>
        /// <param name="member">The member to remove.</param>
        /// <returns><c>true</c> if the member was removed; otherwise <c>false</c>.</returns>
        protected bool Remove(IMember member)
        {
            return _members.Remove(member.Name);
        }

        /// <summary>
        /// Adds the specified <paramref name="member"/> to the set.
        /// </summary>
        /// <param name="member">The member to add.</param>
        /// <exception cref="ArgumentException">
        /// A member with the same name was already added to this set.
        /// </exception>
        protected void Add(IMember member)
        {
            try
            {
                _members.Add(member.Name, member);
            }
            catch (ArgumentException)
            {
                throw NewMemberAlreadyAddedException(member.Name);
            }
        }

        /// <inheritdoc />
        public void AddErrorMessagesTo(IErrorMessageConsumer consumer)
        {
            foreach (var member in _members.Values)
            {
                member.AddErrorMessagesTo(consumer);
            }
        }

        #region [====== IEnumerable ======]

        /// <inheritdoc />
        public IEnumerator<IMember> GetEnumerator()
        {
            return _members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static Exception NewMemberAlreadyAddedException(string memberName)
        {
            var messageFormat = ExceptionMessages.MemberSet_MemberAlreadyAdded;
            var message = string.Format(messageFormat, memberName);
            return new ArgumentException(message);
        }
    }
}

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
        private readonly LinkedList<string> _parentMembers;

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
            _parentMembers = new LinkedList<string>();
        }

        /// <summary>
        /// The <see cref="IErrorMessageConsumer" /> that was specified to collect errors as
        /// new constraints are being added to certain members.
        /// </summary>
        public IErrorMessageConsumer Consumer
        {
            get { return _consumer; }
        }

        #region [====== Push & Pop Parent ======]

        internal void PushParent(string parentMemberName)
        {
            _parentMembers.AddLast(parentMemberName);
        }

        internal void PopParent()
        {
            _parentMembers.RemoveLast();
        }

        #endregion

        #region [====== StartToAddConstraintsFor ======]

        /// <summary>
        /// Creates and returns a new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to verify.</typeparam>
        /// <param name="valueExpression">
        /// An expression that returns an instance of <typeparamref name="TValue"/>.
        /// </param>
        /// <returns>A new <see cref="Member{TValue}"/> that can be used to define certain
        /// constraints on <typeparamref name="TValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueExpression"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="valueExpression"/> is not a <see cref="MemberExpression" /> pointing to a field or property.
        /// </exception>
        public Member<TValue> StartToAddConstraintsFor<TValue>(Expression<Func<TValue>> valueExpression)
        {
            if (valueExpression == null)
            {
                throw new ArgumentNullException("valueExpression");
            }
            return StartToAddConstraintsFor(valueExpression.Compile(), ExtractMemberNameFrom(valueExpression));
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
        public Member<TValue> StartToAddConstraintsFor<TValue>(Func<TValue> valueFactory, string name)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            var member = new Member<TValue>(this, valueFactory, name, DetermineParentName());

            Put(member);

            return member;
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
        public Member<TValue> StartToAddConstraintsFor<TValue>(TValue value, string name)
        {
            return StartToAddConstraintsFor(() => value, name);
        }

        private void Put(IMember newMember)
        {
            IMember oldMember;

            if (_members.TryGetValue(newMember.FullName, out oldMember))
            {
                Replace(oldMember, newMember);
            }
            else
            {
                Add(newMember);
            }
        }

        private string DetermineParentName()
        {
            if (_parentMembers.Count == 0)
            {
                return null;
            }
            return string.Join(".", _parentMembers);
        }

        private static string ExtractMemberNameFrom(Expression valueExpression)
        {
            var lambdaExpression = (LambdaExpression) valueExpression;
            MemberExpression memberExpression;

            if (TryCastToMemberExpression(lambdaExpression, out memberExpression))
            {
                return memberExpression.Member.Name;
            }            
            throw NewExpressionNotSupportedException(valueExpression);
        }

        private static bool TryCastToMemberExpression(LambdaExpression lambdaExpression, out MemberExpression memberExpression)
        {
            var unaryExpression = lambdaExpression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = (MemberExpression) unaryExpression.Operand;
                return true;
            }
            memberExpression = lambdaExpression.Body as MemberExpression;
            return memberExpression != null;
        }

        #endregion

        #region [====== Add, Remove & Replace ======]

        /// <summary>
        /// Replaces <paramref name="oldMember"/> by the specified <paramref name="newMember"/>.
        /// </summary>
        /// <param name="oldMember">The member to remove.</param>
        /// <param name="newMember">The member to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldMember"/> or <paramref name="newMember"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="oldMember"/> and <paramref name="newMember"/> do not have the same name.
        /// </exception>
        protected internal void Replace(IMember oldMember, IMember newMember)
        {
            if (ReferenceEquals(oldMember, null))
            {
                throw new ArgumentNullException("oldMember");
            }
            if (ReferenceEquals(newMember, null))
            {
                throw new ArgumentNullException("newMember");
            }
            if (ReferenceEquals(oldMember, newMember))
            {
                return;
            }
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        protected bool Remove(IMember member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return _members.Remove(member.FullName);
        }

        /// <summary>
        /// Adds the specified <paramref name="member"/> to the set.
        /// </summary>
        /// <param name="member">The member to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A member with the same name was already added to this set.
        /// </exception>
        protected void Add(IMember member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            try
            {
                _members.Add(member.FullName, member);
            }
            catch (ArgumentException)
            {
                throw NewMemberAlreadyAddedException(member.FullName);
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

        #endregion

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

        private static Exception NewExpressionNotSupportedException(Expression valueExpression)
        {
            var messageFormat = ExceptionMessages.MemberSet_UnsupportedExpression;
            var message = string.Format(messageFormat, valueExpression.NodeType);
            return new ArgumentException(message, "valueExpression");
        }
    }
}

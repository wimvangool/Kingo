using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a set of <see cref="IMember"/> instances that are validated by adding run-time constraints.
    /// </summary>
    /// <typeparam name="T">Type of the object the constraints are added for.</typeparam>
    public class MemberConstraintSet<T> : IMemberConstraintSet<T>, IErrorMessageProducer<T>, IEnumerable<IMemberConstraint<T>>
    {               
        private readonly Dictionary<string, IMemberConstraint<T>> _membersConstraints;
        private readonly LinkedList<string> _parentMembers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintSet{T}" /> class.
        /// </summary>        
        public MemberConstraintSet()
        {                        
            _membersConstraints = new Dictionary<string, IMemberConstraint<T>>();
            _parentMembers = new LinkedList<string>();
        }                

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(Environment.NewLine, _membersConstraints.Values);
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

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }
            return VerifyThat(memberExpression.Compile(), ExtractMemberNameFrom(memberExpression));
        }

        /// <inheritdoc />
        public IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> memberValueFactory, string memberName)
        {
            var member = new Member<T, TValue>(memberValueFactory, memberName, DetermineParentName());
            var memberConstraint = new MemberConstraint<T, TValue, TValue>(this, member, new TrueConstraint<TValue>());

            Put(memberConstraint);

            return memberConstraint;
        }            

        private void Put(IMemberConstraint<T> newConstraint)
        {
            IMemberConstraint<T> oldConstraint;

            if (_membersConstraints.TryGetValue(newConstraint.Member.FullName, out oldConstraint))
            {
                Replace(oldConstraint, newConstraint);
            }
            else
            {
                Add(newConstraint);
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
        /// Replaces <paramref name="oldConstraint"/> by the specified <paramref name="newConstraint"/>.
        /// </summary>
        /// <param name="oldConstraint">The member to remove.</param>
        /// <param name="newConstraint">The member to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="oldConstraint"/> or <paramref name="newConstraint"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="oldConstraint"/> and <paramref name="newConstraint"/> do not have the same name.
        /// </exception>
        protected internal void Replace(IMemberConstraint<T> oldConstraint, IMemberConstraint<T> newConstraint)
        {
            if (ReferenceEquals(oldConstraint, null))
            {
                throw new ArgumentNullException("oldConstraint");
            }
            if (ReferenceEquals(newConstraint, null))
            {
                throw new ArgumentNullException("newConstraint");
            }
            if (ReferenceEquals(oldConstraint, newConstraint))
            {
                return;
            }
            if (Remove(oldConstraint))
            {
                Add(newConstraint);
            }            
        }

        /// <summary>
        /// Occurs when a constraint was removed from this set.
        /// </summary>
        public event EventHandler<MemberConstraintEventArgs<T>> ConstraintRemoved;

        /// <summary>
        /// Raises the <see cref="ConstraintRemoved"/> event.
        /// </summary>
        /// <param name="arguments">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnConstraintRemoved(MemberConstraintEventArgs<T> arguments)
        {
            ConstraintRemoved.Raise(this, arguments);
        }

        /// <summary>
        /// Removes the specified <paramref name="constraint"/> from the set.
        /// </summary>
        /// <param name="constraint">The member to remove.</param>
        /// <returns><c>true</c> if the member was removed; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        protected bool Remove(IMemberConstraint<T> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (_membersConstraints.Remove(constraint.Member.FullName))
            {
                OnConstraintRemoved(new MemberConstraintEventArgs<T>(constraint));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Occurs when a constraint was added to this set.
        /// </summary>
        public event EventHandler<MemberConstraintEventArgs<T>> ConstraintAdded;

        /// <summary>
        /// Raises the <see cref="ConstraintAdded"/> event.
        /// </summary>
        /// <param name="arguments">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnConstraintAdded(MemberConstraintEventArgs<T> arguments)
        {
            ConstraintAdded.Raise(this, arguments);
        }

        /// <summary>
        /// Adds the specified <paramref name="constraint"/> to the set.
        /// </summary>
        /// <param name="constraint">The member to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A member with the same name was already added to this set.
        /// </exception>
        protected void Add(IMemberConstraint<T> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            try
            {
                _membersConstraints.Add(constraint.Member.FullName, constraint);                
            }
            catch (ArgumentException)
            {
                throw NewMemberAlreadyAddedException(constraint.Member.FullName);
            }
            OnConstraintAdded(new MemberConstraintEventArgs<T>(constraint));
        }        

        /// <inheritdoc />
        public bool HasErrors(T item, IErrorMessageConsumer consumer, IFormatProvider formatProvider = null)
        {
            var hasAddedErrorMessages = false;

            foreach (var member in _membersConstraints.Values)
            {
                hasAddedErrorMessages |= member.HasErrors(item, consumer, formatProvider);
            }
            return hasAddedErrorMessages;
        }

        #endregion

        #region [====== IEnumerable ======]

        /// <inheritdoc />
        public IEnumerator<IMemberConstraint<T>> GetEnumerator()
        {
            return _membersConstraints.Values.GetEnumerator();
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

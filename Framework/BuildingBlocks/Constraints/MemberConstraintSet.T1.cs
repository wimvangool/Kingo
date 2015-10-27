using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a set of <see cref="IMember"/> instances that are validated by adding run-time constraints.
    /// </summary>
    /// <typeparam name="TMessage">Type of the object the constraints are added for.</typeparam>
    public class MemberConstraintSet<TMessage> : IMemberConstraintSet<TMessage>, IErrorMessageWriter<TMessage>
    {
        #region [====== ChildConstraintSet  & ChildConstraint ======]

        private sealed class ChildConstraintSet<TOriginal, TResult> : IErrorMessageWriter<TOriginal>, IMemberConstraintSet<TResult>            
        {
            private readonly MemberConstraintSet<TOriginal> _parentSet;
            private readonly MemberConstraintSet<TResult> _childSet;
            private readonly Member<TOriginal, TResult> _member;

            internal ChildConstraintSet(MemberConstraintSet<TOriginal> parentSet, Member<TOriginal, TResult> member)
            {
                _parentSet = parentSet;
                _childSet = new MemberConstraintSet<TResult>(AddChildMemberName(parentSet._parentNames, member.Name));
                _childSet.ConstraintRemoved += HandleConstraintRemoved;
                _childSet.ConstraintAdded += HandleConstraintAdded;
                _member = member;
            }

            private void HandleConstraintRemoved(object sender, MemberConstraintEventArgs<TResult> arguments)
            {
                _parentSet.OnConstraintRemoved(Convert(arguments));
            }

            private void HandleConstraintAdded(object sender, MemberConstraintEventArgs<TResult> arguments)
            {
                _parentSet.OnConstraintAdded(Convert(arguments));
            }

            private MemberConstraintEventArgs<TOriginal> Convert(MemberConstraintEventArgs<TResult> arguments)
            {
                return new MemberConstraintEventArgs<TOriginal>(new ChildConstraint<TOriginal,TResult>(arguments.MemberConstraint, _member));
            }
                
            IMemberConstraint<TResult, TValue> IMemberConstraintSet<TResult>.VerifyThat<TValue>(Expression<Func<TResult, TValue>> memberExpression)
            {
                return _childSet.VerifyThat(memberExpression);
            }

            IMemberConstraint<TResult, TValue> IMemberConstraintSet<TResult>.VerifyThat<TValue>(Func<TResult, TValue> memberValueFactory, string memberName)
            {
                return _childSet.VerifyThat(memberValueFactory, memberName);
            }

            public bool WriteErrorMessages(TOriginal item, IErrorMessageReader reader)
            {
                return _childSet.WriteErrorMessages(_member.GetValue(item), reader);
            }
           
            private static string[] AddChildMemberName(IReadOnlyList<string> parentNames, string memberName)
            {
                var parentNamesPlusMemberName = new string[parentNames.Count + 1];

                for (int index = 0; index < parentNames.Count; index++)
                {
                    parentNamesPlusMemberName[index] = parentNames[index];
                }
                parentNamesPlusMemberName[parentNames.Count] = memberName;

                return parentNamesPlusMemberName;
            }
        }

        private sealed class ChildConstraint<TOriginal, TResult> : IMemberConstraint<TOriginal>
        {
            private readonly IMemberConstraint<TResult> _childMemberConstraint;
            private readonly Member<TOriginal, TResult> _member;

            internal ChildConstraint(IMemberConstraint<TResult> childMemberConstraint, Member<TOriginal, TResult> member)
            {
                _childMemberConstraint = childMemberConstraint;
                _member = member;
            }

            public IMember Member
            {
                get { return _childMemberConstraint.Member; }
            }

            public bool WriteErrorMessages(TOriginal item, IErrorMessageReader reader)
            {
                return _childMemberConstraint.WriteErrorMessages(_member.GetValue(item), reader);
            }
        }

        #endregion

        private readonly Dictionary<string, LinkedList<IMemberConstraint<TMessage>>> _membersConstraints;
        private readonly Dictionary<string, IErrorMessageWriter<TMessage>> _childConstraintSets;
        private readonly string[] _parentNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintSet{T}" /> class.
        /// </summary>        
        public MemberConstraintSet()
            : this(new string[0]) { }
        
        private MemberConstraintSet(string[] parentNames)
        {
            _membersConstraints = new Dictionary<string, LinkedList<IMemberConstraint<TMessage>>>();
            _childConstraintSets = new Dictionary<string, IErrorMessageWriter<TMessage>>();
            _parentNames = parentNames;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(Environment.NewLine, _membersConstraints.Values);
        }

        #region [====== Push & Pop Parent ======]

        internal void AddChildMemberConstraints<TResult>(Action<IMemberConstraintSet<TResult>> innerConstraintFactory, Member<TMessage, TResult> member)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException("innerConstraintFactory");
            }
            var childConstraintSet = new ChildConstraintSet<TMessage, TResult>(this, member);

            innerConstraintFactory.Invoke(childConstraintSet);

            _childConstraintSets.Add(member.FullName, childConstraintSet);            
        }        

        #endregion

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Expression<Func<TMessage, TValue>> memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }
            return VerifyThat(memberExpression.Compile(), memberExpression.ExtractMemberName());
        }

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> memberValueFactory, string memberName)
        {
            var member = new Member<TMessage, TValue>(_parentNames, memberName, memberValueFactory);            
            var memberConstraint = new MemberConstraint<TMessage, TValue, TValue>(this, CreateConstraintFactory(member));

            Add(memberConstraint);

            return memberConstraint;
        }        
    
        private static MemberConstraintFactory<TMessage, TValue, TValue> CreateConstraintFactory<TValue>(Member<TMessage, TValue> member)
        {
            return new MemberConstraintFactory<TMessage, TValue, TValue>(member, message => new NullConstraint<TValue>().MapInputToOutput());
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
        protected internal void Replace(IMemberConstraint<TMessage> oldConstraint, IMemberConstraint<TMessage> newConstraint)
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
            if (Remove(oldConstraint, false))
            {
                Add(newConstraint);
            }            
        }

        /// <summary>
        /// Occurs when a constraint was removed from this set.
        /// </summary>
        public event EventHandler<MemberConstraintEventArgs<TMessage>> ConstraintRemoved;

        /// <summary>
        /// Raises the <see cref="ConstraintRemoved"/> event.
        /// </summary>
        /// <param name="arguments">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnConstraintRemoved(MemberConstraintEventArgs<TMessage> arguments)
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
        protected bool Remove(IMemberConstraint<TMessage> constraint)
        {
            return Remove(constraint, true);
        }

        private bool Remove(IMemberConstraint<TMessage> constraint, bool removeEmptyList)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            var member = constraint.Member.FullName;
            LinkedList<IMemberConstraint<TMessage>> existingConstraints;

            if (_membersConstraints.TryGetValue(member, out existingConstraints) && existingConstraints.Remove(constraint))
            {
                if (removeEmptyList && existingConstraints.Count == 0)
                {
                    _membersConstraints.Remove(member);
                }
                OnConstraintRemoved(new MemberConstraintEventArgs<TMessage>(constraint));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Occurs when a constraint was added to this set.
        /// </summary>
        public event EventHandler<MemberConstraintEventArgs<TMessage>> ConstraintAdded;

        /// <summary>
        /// Raises the <see cref="ConstraintAdded"/> event.
        /// </summary>
        /// <param name="arguments">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="arguments"/> is <c>null</c>.
        /// </exception>
        protected virtual void OnConstraintAdded(MemberConstraintEventArgs<TMessage> arguments)
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
        protected void Add(IMemberConstraint<TMessage> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            var member = constraint.Member.FullName;
            LinkedList<IMemberConstraint<TMessage>> existingConstraints;

            if (!_membersConstraints.TryGetValue(member, out existingConstraints))
            {
                _membersConstraints.Add(member, existingConstraints = new LinkedList<IMemberConstraint<TMessage>>());
            }
            existingConstraints.AddLast(constraint);

            OnConstraintAdded(new MemberConstraintEventArgs<TMessage>(constraint));
        }              

        /// <inheritdoc />
        public bool WriteErrorMessages(TMessage message, IErrorMessageReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            var hasAddedErrorMessages = false;

            foreach (var member in MemberConstraints())
            {
                hasAddedErrorMessages |= member.WriteErrorMessages(message, reader);
            }
            return hasAddedErrorMessages;
        }

        private IEnumerable<IErrorMessageWriter<TMessage>> MemberConstraints()
        {
            return _membersConstraints.Values.SelectMany(constraint => constraint).Concat(_childConstraintSets.Values);
        }

        #endregion                           
    }
}

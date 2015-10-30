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
                _childSet = new MemberConstraintSet<TResult>(_parentSet._haltOnFirstError, AddChildMemberName(parentSet._parentNames, member.Name));
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

            IMemberConstraint<TResult, TResult> IMemberConstraintSet<TResult>.VerifyThatInstance()
            {
                return _childSet.VerifyThatInstance();
            }
                
            IMemberConstraint<TResult, TValue> IMemberConstraintSet<TResult>.VerifyThat<TValue>(Expression<Func<TResult, TValue>> fieldOrPropertyExpression)
            {
                return _childSet.VerifyThat(fieldOrPropertyExpression);
            }

            IMemberConstraint<TResult, TValue> IMemberConstraintSet<TResult>.VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, string fieldOrPropertyName)
            {
                return _childSet.VerifyThat(fieldOrProperty, fieldOrPropertyName);
            }

            IMemberConstraint<TResult, TValue> IMemberConstraintSet<TResult>.VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, Identifier fieldOrPropertyName)
            {
                return _childSet.VerifyThat(fieldOrProperty, fieldOrPropertyName);
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

        private readonly LinkedList<IMemberConstraint<TMessage>> _messageConstraints;
        private readonly Dictionary<string, LinkedList<IMemberConstraint<TMessage>>> _membersConstraints;
        private readonly Dictionary<string, IErrorMessageWriter<TMessage>> _childConstraintSets;
        private readonly string[] _parentNames;
        private readonly bool _haltOnFirstError;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintSet{T}" /> class.
        /// </summary>        
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this constraint set should stop evaluating constraints once a constraint has failed.
        /// </param>
        public MemberConstraintSet(bool haltOnFirstError = false)
            : this(haltOnFirstError, new string[0]) { }
        
        private MemberConstraintSet(bool haltOnFirstError, string[] parentNames)
        {
            _messageConstraints = new LinkedList<IMemberConstraint<TMessage>>();
            _membersConstraints = new Dictionary<string, LinkedList<IMemberConstraint<TMessage>>>();
            _childConstraintSets = new Dictionary<string, IErrorMessageWriter<TMessage>>();
            _parentNames = parentNames;
            _haltOnFirstError = haltOnFirstError;
        }

        /// <summary>
        /// Indicates whether or not this constraint set stops evaluating constraints once a constraint has failed.
        /// </summary>
        public bool HaltsOnFirstError
        {
            get { return _haltOnFirstError; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} constraint(s)", _membersConstraints.Count + _childConstraintSets.Count);
        }

        #region [====== ChildMemberConstraints ======]

        internal void AddChildMemberConstraints<TResult>(Action<IMemberConstraintSet<TResult>> innerConstraintFactory, Member<TMessage, TResult> member)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException("innerConstraintFactory");
            }
            var childConstraintSet = new ChildConstraintSet<TMessage, TResult>(this, member);

            innerConstraintFactory.Invoke(childConstraintSet);

            _childConstraintSets.Add(member.Key, childConstraintSet);            
        }        

        #endregion

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TMessage> VerifyThatInstance()
        {
            return AddNullConstraintFor(new Member<TMessage, TMessage>(_parentNames, null, message => message));
        }

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Expression<Func<TMessage, TValue>> fieldOrPropertyExpression)
        {
            if (fieldOrPropertyExpression == null)
            {
                throw new ArgumentNullException("fieldOrPropertyExpression");
            }
            return VerifyThat(fieldOrPropertyExpression.Compile(), fieldOrPropertyExpression.ExtractMemberName());
        }

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> fieldOrProperty, string fieldOrPropertyName)
        {
            return VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }

        /// <inheritdoc />
        public IMemberConstraint<TMessage, TValue> VerifyThat<TValue>(Func<TMessage, TValue> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return AddNullConstraintFor(new Member<TMessage, TValue>(_parentNames, fieldOrPropertyName.ToString(), fieldOrProperty));            
        }

        private IMemberConstraint<TMessage, TValue> AddNullConstraintFor<TValue>(Member<TMessage, TValue> member)
        {
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
            var memberKey = constraint.Member.Key;
            if (memberKey == null)
            {
                if (_messageConstraints.Remove(constraint))
                {
                    OnConstraintRemoved(new MemberConstraintEventArgs<TMessage>(constraint));
                    return true;
                }
            }
            else
            {
                LinkedList<IMemberConstraint<TMessage>> existingConstraints;

                if (_membersConstraints.TryGetValue(memberKey, out existingConstraints) && existingConstraints.Remove(constraint))
                {
                    if (removeEmptyList && existingConstraints.Count == 0)
                    {
                        _membersConstraints.Remove(memberKey);
                    }
                    OnConstraintRemoved(new MemberConstraintEventArgs<TMessage>(constraint));
                    return true;
                }
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
            var memberKey = constraint.Member.Key;
            var existingConstraints = GetOrAddConstraintListFor(memberKey);

            existingConstraints.AddLast(constraint);

            OnConstraintAdded(new MemberConstraintEventArgs<TMessage>(constraint));
        }
 
        private LinkedList<IMemberConstraint<TMessage>> GetOrAddConstraintListFor(string memberKey)
        {
            if (memberKey == null)
            {
                return _messageConstraints; ;
            }
            LinkedList<IMemberConstraint<TMessage>> existingConstraints;

            if (!_membersConstraints.TryGetValue(memberKey, out existingConstraints))
            {
                _membersConstraints.Add(memberKey, existingConstraints = new LinkedList<IMemberConstraint<TMessage>>());
            }
            return existingConstraints;
        }
        
        #endregion

        #region [====== WriteErrorMessages ======]

        /// <inheritdoc />
        public bool WriteErrorMessages(TMessage message, IErrorMessageReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (_haltOnFirstError)
            {
                foreach (var constraint in AllConstraints())
                {
                    if (constraint.WriteErrorMessages(message, reader))
                    {
                        return true;
                    }
                }
                return false;
            }
            var hasAddedErrorMessages = false;

            foreach (var constraint in AllConstraints())
            {
                hasAddedErrorMessages |= constraint.WriteErrorMessages(message, reader);
            }
            return hasAddedErrorMessages;          
        }

        private IEnumerable<IErrorMessageWriter<TMessage>> AllConstraints()
        {
            return _messageConstraints
                .Concat(_membersConstraints.Values.SelectMany(constraint => constraint)
                .Concat(_childConstraintSets.Values));
        }

        #endregion                           
    }
}

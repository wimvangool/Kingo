using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a set of constraints declared or added for (members of) a specific type.
    /// </summary>
    /// <typeparam name="T">Type of the object the constraints are added for.</typeparam>
    public class MemberConstraintSet<T> : IMemberConstraintSet<T>, IErrorMessageWriter<T>
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
                _childSet = new MemberConstraintSet<TResult>(_parentSet._haltOnFirstError, parentSet._parentNames.Add(member.Name));
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

            #region [====== VerifyThatInstance ======]

            IMemberConstraint<TResult, TResult> IMemberConstraintSet<TResult>.VerifyThatInstance()
            {
                return _childSet.VerifyThatInstance();
            }

            #endregion

            #region [====== VerifyThat ======]

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

            #endregion

            #region [====== VerifyThatCollection ======]

            IMemberConstraint<TResult, IEnumerable<TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TValue>(Expression<Func<TResult, IEnumerable<TValue>>> fieldOrProperty)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty);
            }

            IMemberConstraint<TResult, IEnumerable<TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TValue>(Func<TResult, IEnumerable<TValue>> fieldOrProperty, string fieldOrPropertyName)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty, fieldOrPropertyName);
            }

            IMemberConstraint<TResult, IEnumerable<TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TValue>(Func<TResult, IEnumerable<TValue>> fieldOrProperty, Identifier fieldOrPropertyName)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty, fieldOrPropertyName);
            }            

            #endregion

            #region [====== VerifyThatDictionary ======]

            IMemberConstraint<TResult, IReadOnlyDictionary<TKey, TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TKey, TValue>(Expression<Func<TResult, IReadOnlyDictionary<TKey, TValue>>> fieldOrProperty)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty);
            }

            IMemberConstraint<TResult, IReadOnlyDictionary<TKey, TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TKey, TValue>(Func<TResult, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, string fieldOrPropertyName)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty, fieldOrPropertyName);
            }

            IMemberConstraint<TResult, IReadOnlyDictionary<TKey, TValue>> IMemberConstraintSet<TResult>.VerifyThatCollection<TKey, TValue>(Func<TResult, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, Identifier fieldOrPropertyName)
            {
                return _childSet.VerifyThatCollection(fieldOrProperty, fieldOrPropertyName);
            }

            #endregion

            #region [====== WriteErrorMessages ======]

            public bool WriteErrorMessages(TOriginal item, IErrorMessageReader reader)
            {
                return _childSet.WriteErrorMessages(_member.GetValue(item), reader);
            }

            #endregion            
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

        private readonly LinkedList<IMemberConstraint<T>> _instanceConstraints;
        private readonly Dictionary<string, LinkedList<IMemberConstraint<T>>> _membersConstraints;
        private readonly Dictionary<string, IErrorMessageWriter<T>> _childConstraintSets;
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
            _instanceConstraints = new LinkedList<IMemberConstraint<T>>();
            _membersConstraints = new Dictionary<string, LinkedList<IMemberConstraint<T>>>();
            _childConstraintSets = new Dictionary<string, IErrorMessageWriter<T>>();
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

        internal void AddChildMemberConstraints<TResult>(Action<IMemberConstraintSet<TResult>> innerConstraintFactory, Member<T, TResult> member)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException("innerConstraintFactory");
            }
            var childConstraintSet = new ChildConstraintSet<T, TResult>(this, member);

            innerConstraintFactory.Invoke(childConstraintSet);

            _childConstraintSets.Add(member.Key, childConstraintSet);            
        }        

        #endregion

        #region [====== VerifyThatInstance ======]

        /// <inheritdoc />
        public IMemberConstraint<T, T> VerifyThatInstance()
        {
            return AddNullConstraintFor(new Member<T, T>(_parentNames, null, message => message));
        }

        #endregion

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraint<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            return VerifyThat(fieldOrProperty.Compile(), fieldOrProperty.ExtractMemberName());
        }

        /// <inheritdoc />
        public IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, string fieldOrPropertyName)
        {
            return VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }

        /// <inheritdoc />
        public IMemberConstraint<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return AddNullConstraintFor(fieldOrProperty, fieldOrPropertyName);            
        }

        #endregion

        #region [====== VerifyThatCollection (IEnumerable<>) ======]

        public IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Expression<Func<T, IEnumerable<TValue>>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            return VerifyThatCollection(fieldOrProperty.Compile(), fieldOrProperty.ExtractMemberName());
        }

        public IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Func<T, IEnumerable<TValue>> fieldOrProperty, string fieldOrPropertyName)
        {
            return VerifyThatCollection(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }

        public IMemberConstraint<T, IEnumerable<TValue>> VerifyThatCollection<TValue>(Func<T, IEnumerable<TValue>> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }            
            return AddNullConstraintFor(fieldOrProperty, fieldOrPropertyName);
        }
       
        #endregion

        #region [====== VerifyThatDCollection (IReadOnlyDictionary<,>) ======]

        public IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Expression<Func<T, IReadOnlyDictionary<TKey, TValue>>> fieldOrProperty)
        {
            if (fieldOrProperty == null)
            {
                throw new ArgumentNullException("fieldOrProperty");
            }
            return VerifyThatCollection(fieldOrProperty.Compile(), fieldOrProperty.ExtractMemberName());
        }

        public IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Func<T, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, string fieldOrPropertyName)
        {
            return VerifyThatCollection(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }

        public IMemberConstraint<T, IReadOnlyDictionary<TKey, TValue>> VerifyThatCollection<TKey, TValue>(Func<T, IReadOnlyDictionary<TKey, TValue>> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return AddNullConstraintFor(fieldOrProperty, fieldOrPropertyName);
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
            if (Remove(oldConstraint, false))
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
            return Remove(constraint, true);
        }

        private bool Remove(IMemberConstraint<T> constraint, bool removeEmptyList)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            var memberKey = constraint.Member.Key;
            if (memberKey == null)
            {
                if (_instanceConstraints.Remove(constraint))
                {
                    OnConstraintRemoved(new MemberConstraintEventArgs<T>(constraint));
                    return true;
                }
            }
            else
            {
                LinkedList<IMemberConstraint<T>> existingConstraints;

                if (_membersConstraints.TryGetValue(memberKey, out existingConstraints) && existingConstraints.Remove(constraint))
                {
                    if (removeEmptyList && existingConstraints.Count == 0)
                    {
                        _membersConstraints.Remove(memberKey);
                    }
                    OnConstraintRemoved(new MemberConstraintEventArgs<T>(constraint));
                    return true;
                }
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

        private IMemberConstraint<T, TValue> AddNullConstraintFor<TValue>(Func<T, TValue> valueFactory, Identifier name)
        {
            return AddNullConstraintFor(new Member<T, TValue>(_parentNames, name.ToString(), valueFactory));
        }

        private IMemberConstraint<T, TValue> AddNullConstraintFor<TValue>(Member<T, TValue> member)
        {
            Func<T, IFilter<TValue, TValue>> nullConstraint = message => new NullConstraint<TValue>().MapInputToOutput();
            var memberConstraintFactory = new MemberConstraintFactory<T, TValue, TValue>(member, nullConstraint);
            var memberConstraint = new MemberConstraint<T, TValue, TValue>(this, memberConstraintFactory);

            Add(memberConstraint);

            return memberConstraint;
        }           

        /// <summary>
        /// Adds the specified <paramref name="constraint"/> to the set.
        /// </summary>
        /// <param name="constraint">The member to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>        
        protected void Add(IMemberConstraint<T> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            var memberKey = constraint.Member.Key;
            var existingConstraints = GetOrAddConstraintListFor(memberKey);

            existingConstraints.AddLast(constraint);

            OnConstraintAdded(new MemberConstraintEventArgs<T>(constraint));
        }
 
        private LinkedList<IMemberConstraint<T>> GetOrAddConstraintListFor(string memberKey)
        {
            if (memberKey == null)
            {
                return _instanceConstraints; ;
            }
            LinkedList<IMemberConstraint<T>> existingConstraints;

            if (!_membersConstraints.TryGetValue(memberKey, out existingConstraints))
            {
                _membersConstraints.Add(memberKey, existingConstraints = new LinkedList<IMemberConstraint<T>>());
            }
            return existingConstraints;
        }
        
        #endregion

        #region [====== WriteErrorMessages ======]

        /// <inheritdoc />
        public bool WriteErrorMessages(T instance, IErrorMessageReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            return _haltOnFirstError
                ? WriteOnlyFirstError(instance, reader)
                : WriteAllErrors(instance, reader);                   
        }

        private bool WriteOnlyFirstError(T instance, IErrorMessageReader reader)
        {
            foreach (var constraint in MemberConstraints().Concat(_instanceConstraints))
            {
                if (constraint.WriteErrorMessages(instance, reader))
                {
                    return true;
                }
            }
            return false;
        }

        private bool WriteAllErrors(T instance, IErrorMessageReader reader)
        {
            var hasAddedErrorMessages = false;

            foreach (var constraint in MemberConstraints())
            {
                hasAddedErrorMessages |= constraint.WriteErrorMessages(instance, reader);
            }
            if (!hasAddedErrorMessages)
            {
                foreach (var constraint in _instanceConstraints)
                {
                    hasAddedErrorMessages |= constraint.WriteErrorMessages(instance, reader);
                }
            }
            return hasAddedErrorMessages; 
        }

        private IEnumerable<IErrorMessageWriter<T>> MemberConstraints()
        {
            return _membersConstraints.Values.SelectMany(constraint => constraint).Concat(_childConstraintSets.Values);
        }

        #endregion
    }
}

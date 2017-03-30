using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a set of constraints declared or added for (members of) a specific type.
    /// </summary>
    /// <typeparam name="T">Type of the object the constraints are added for.</typeparam>
    public class MemberConstraintSet<T> : IMemberConstraintSet<T>, IErrorMessageWriter<T>
    {        
        private readonly LinkedList<IMemberConstraintBuilder<T>> _instanceConstraints;
        private readonly Dictionary<Guid, IMemberConstraintBuilder<T>> _membersConstraints;
        private readonly List<IErrorMessageWriter<T>> _childConstraintSets;                

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintSet{T}" /> class.
        /// </summary>                
        public MemberConstraintSet()
        {
            _instanceConstraints = new LinkedList<IMemberConstraintBuilder<T>>();
            _membersConstraints = new Dictionary<Guid, IMemberConstraintBuilder<T>>();
            _childConstraintSets = new List<IErrorMessageWriter<T>>();                        
        }        

        /// <inheritdoc />
        public override string ToString() =>
            $"{_membersConstraints.Count + _childConstraintSets.Count} constraint(s)";

        #region [====== ChildMemberConstraints ======]

        private sealed class ChildConstraintSet<TOriginal, TResult> : IErrorMessageWriter<TOriginal>, IMemberConstraintSet<TResult>
        {
            private readonly MemberConstraintSet<TResult> _childSet;
            private readonly MemberFactory<TOriginal, TResult> _memberFactory;
            private TOriginal _instance;

            internal ChildConstraintSet(MemberConstraintSet<TOriginal> parentSet, MemberFactory<TOriginal, TResult> memberFactory)
            {
                _childSet = new MemberConstraintSet<TResult>();
                _memberFactory = memberFactory;
            }

            public IMemberConstraintBuilder<TResult, TResult> VerifyThatInstance() =>
                VerifyThat(value => value, null);

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Expression<Func<TResult, TValue>> fieldOrPropertyExpression) =>
                new VerifyThatExpressionDecoder<TResult, TValue>(this, fieldOrPropertyExpression);

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, string fieldOrPropertyName) =>
                VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, Identifier fieldOrPropertyName) =>
                _childSet.AddMemberConstraintFor(_memberFactory.Transform(fieldOrProperty, fieldOrPropertyName, () => _instance));

            public bool WriteErrorMessages(TOriginal instance, IErrorMessageCollection reader, bool haltOnFirstError)
            {
                _instance = instance;

                try
                {
                    return _childSet.WriteErrorMessages(_memberFactory.CreateMember(instance).Value, reader, haltOnFirstError);
                }
                finally
                {
                    _instance = default(TOriginal);
                }                
            }
        }               

        internal void AddChildMemberConstraints<TResult>(Action<IMemberConstraintSet<TResult>> innerConstraintFactory, MemberFactory<T, TResult> memberFactory)
        {
            if (innerConstraintFactory == null)
            {
                throw new ArgumentNullException(nameof(innerConstraintFactory));
            }
            var childConstraintSet = new ChildConstraintSet<T, TResult>(this, memberFactory);

            innerConstraintFactory.Invoke(childConstraintSet);

            _childConstraintSets.Add(childConstraintSet);            
        }        

        #endregion

        #region [====== VerifyThatInstance ======]

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, T> VerifyThatInstance() =>
            AddInstanceConstraintFor(new MemberFactory<T, T>(message => message));

        #endregion

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> fieldOrProperty) =>
            new VerifyThatExpressionDecoder<T, TValue>(this, fieldOrProperty);

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, string fieldOrPropertyName) =>
            VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, Identifier fieldOrPropertyName) =>
            AddMemberConstraintFor(new MemberFactory<T, TValue>(fieldOrProperty, fieldOrPropertyName));

        #endregion

        #region [====== Add & Replace ======]

        internal void Replace(IMemberConstraintBuilder<T> oldConstraint, IMemberConstraintBuilder<T> newConstraint)
        {
            if (oldConstraint.Key.Equals(Guid.Empty))
            {
                _instanceConstraints.Remove(oldConstraint);
                _instanceConstraints.AddLast(newConstraint);
            }
            else
            {
                _membersConstraints[oldConstraint.Key] = newConstraint;
            }
        }

        private IMemberConstraintBuilder<T, TValue> AddInstanceConstraintFor<TValue>(MemberFactory<T, TValue> member)
        {
            var key = Guid.Empty;
            var constraint = CreateNullConstraintFor(key, member);

            _instanceConstraints.AddLast(constraint);

            return constraint;
        }
        
        private IMemberConstraintBuilder<T, TValue> AddMemberConstraintFor<TValue>(MemberFactory<T, TValue> member)
        {
            var key = Guid.NewGuid();
            var constraint = CreateNullConstraintFor(key, member);

            _membersConstraints.Add(key, constraint);

            return constraint;
        }

        private IMemberConstraintBuilder<T, TValue> CreateNullConstraintFor<TValue>(Guid key, MemberFactory<T, TValue> member)
        {
            Func<T, IFilter<TValue, TValue>> nullConstraint = message => new NullConstraint<TValue>().MapInputToOutput();
            var memberConstraintFactory = new MemberConstraintFactory<T, TValue, TValue>(member, nullConstraint);
            return new MemberConstraintBuilder<T, TValue, TValue>(key, this, memberConstraintFactory);            
        }                   
        
        #endregion

        #region [====== WriteErrorMessages ======]

        /// <inheritdoc />
        public bool WriteErrorMessages(T instance, IErrorMessageCollection reader, bool haltOnFirstError = false)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            return haltOnFirstError
                ? WriteOnlyFirstError(instance, reader)
                : WriteAllErrors(instance, reader);                   
        }

        private bool WriteOnlyFirstError(T instance, IErrorMessageCollection reader)
        {
            foreach (var constraint in MemberConstraints().Concat(_instanceConstraints))
            {
                if (constraint.WriteErrorMessages(instance, reader, true))
                {
                    return true;
                }
            }
            return false;
        }

        private bool WriteAllErrors(T instance, IErrorMessageCollection reader)
        {
            var hasAddedErrorMessages = false;

            foreach (var constraint in MemberConstraints())
            {
                hasAddedErrorMessages |= constraint.WriteErrorMessages(instance, reader, false);
            }
            if (!hasAddedErrorMessages)
            {
                foreach (var constraint in _instanceConstraints)
                {
                    hasAddedErrorMessages |= constraint.WriteErrorMessages(instance, reader, false);
                }
            }
            return hasAddedErrorMessages; 
        }

        private IEnumerable<IErrorMessageWriter<T>> MemberConstraints() =>
            _membersConstraints.Values.Concat(_childConstraintSets);

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kingo.Constraints.Decoders;

namespace Kingo.Constraints
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
        private readonly bool _haltOnFirstError;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintSet{T}" /> class.
        /// </summary>        
        /// <param name="haltOnFirstError">
        /// Indicates whether or not this constraint set should stop evaluating constraints once a constraint has failed.
        /// </param>
        public MemberConstraintSet(bool haltOnFirstError)
        {
            _instanceConstraints = new LinkedList<IMemberConstraintBuilder<T>>();
            _membersConstraints = new Dictionary<Guid, IMemberConstraintBuilder<T>>();
            _childConstraintSets = new List<IErrorMessageWriter<T>>();            
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

        private sealed class ChildConstraintSet<TOriginal, TResult> : IErrorMessageWriter<TOriginal>, IMemberConstraintSet<TResult>
        {
            private readonly MemberConstraintSet<TResult> _childSet;
            private readonly MemberFactory<TOriginal, TResult> _memberFactory;
            private TOriginal _instance;

            internal ChildConstraintSet(MemberConstraintSet<TOriginal> parentSet, MemberFactory<TOriginal, TResult> memberFactory)
            {
                _childSet = new MemberConstraintSet<TResult>(parentSet._haltOnFirstError);
                _memberFactory = memberFactory;
            }

            public IMemberConstraintBuilder<TResult, TResult> VerifyThatInstance()
            {
                return VerifyThat(value => value, null);
            }

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Expression<Func<TResult, TValue>> fieldOrPropertyExpression)
            {
                return new VerifyThatExpressionDecoder<TResult, TValue>(this, fieldOrPropertyExpression);
            }

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, string fieldOrPropertyName)
            {
                return VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
            }

            public IMemberConstraintBuilder<TResult, TValue> VerifyThat<TValue>(Func<TResult, TValue> fieldOrProperty, Identifier fieldOrPropertyName)
            {
                return _childSet.AddMemberConstraintFor(_memberFactory.Transform(fieldOrProperty, fieldOrPropertyName, () => _instance));
            }

            public bool WriteErrorMessages(TOriginal instance, IErrorMessageReader reader)
            {
                _instance = instance;

                try
                {
                    return _childSet.WriteErrorMessages(_memberFactory.CreateMember(instance).Value, reader);
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
        public IMemberConstraintBuilder<T, T> VerifyThatInstance()
        {
            return AddInstanceConstraintFor(new MemberFactory<T, T>(message => message));            
        }

        #endregion

        #region [====== VerifyThat ======]

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Expression<Func<T, TValue>> fieldOrProperty)
        {
            return new VerifyThatExpressionDecoder<T, TValue>(this, fieldOrProperty);
        }

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, string fieldOrPropertyName)
        {
            return VerifyThat(fieldOrProperty, Identifier.ParseOrNull(fieldOrPropertyName));
        }

        /// <inheritdoc />
        public IMemberConstraintBuilder<T, TValue> VerifyThat<TValue>(Func<T, TValue> fieldOrProperty, Identifier fieldOrPropertyName)
        {
            return AddMemberConstraintFor(new MemberFactory<T, TValue>(fieldOrProperty, fieldOrPropertyName));                        
        }

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
        public bool WriteErrorMessages(T instance, IErrorMessageReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
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
            return _membersConstraints.Values.Concat(_childConstraintSets);
        }

        #endregion
    }
}

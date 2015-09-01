using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Constraints
{
    /// <summary>
    /// Represents a builder that can be used to create instances of the <see cref="DelegateConstraint{T, S}" /> class
    /// where no value conversion is required.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public class DelegateConstraintBuilder<TValue> : DelegateConstraintBuilder<TValue, TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraintBuilder{T}" /> class.
        /// </summary>
        public DelegateConstraintBuilder() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraintBuilder{T}" /> class.
        /// </summary>
        /// <param name="constraint">Delegate that represents the constraint logic.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraintBuilder(Func<TValue, bool> constraint)
            : base(constraint, value => value) { }

        /// <summary>
        /// Sets the implementation of the constraint.
        /// </summary>
        /// <param name="constraint">Delegate that represents the constraint logic.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraintBuilder<TValue> WithImplementation(Func<TValue, bool> constraint)
        {
            WithImplementation(constraint, value => value);
            return this;
        }
    }
}

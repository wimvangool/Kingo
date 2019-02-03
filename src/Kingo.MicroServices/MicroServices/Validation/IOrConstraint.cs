using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Validation
{
    /// <summary>
    /// When implemented by a class, represents a composite constraint that represents a logica OR
    /// of several other constraints.
    /// </summary>
    public interface IOrConstraint : IConstraint
    {
        /// <summary>
        /// Instructs the constraint to combine the error messages of all failed child-constraints
        /// into one single error message with the specified <paramref name="mergeFunction"/>.
        /// </summary>
        /// <param name="mergeFunction">
        /// The function that will be used by this constraint to merge all error messages of the child-
        /// constraints into a single error message. The function also receives the value that was
        /// validated by the constraint.
        /// </param>
        /// <returns>The new constraint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mergeFunction"/> is <c>null</c>.
        /// </exception>
        IConstraint CombineErrors(Func<IEnumerable<string>, object, string> mergeFunction);
    }
}

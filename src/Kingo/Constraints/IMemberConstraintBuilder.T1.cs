using System;

namespace Kingo.Constraints
{
    /// <summary>
    /// Represents a builder of constraints for a specific member of a message.
    /// </summary>
    /// <typeparam name="T">Type of the object the error messages are produced for.</typeparam>
    public interface IMemberConstraintBuilder<in T> : IErrorMessageWriter<T>
    {
        /// <summary>
        /// Returns a unique identifier of the constraint.
        /// </summary>
        Guid Key
        {
            get;
        }        
    }
}

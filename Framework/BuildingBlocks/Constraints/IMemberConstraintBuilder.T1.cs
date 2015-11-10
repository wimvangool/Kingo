namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a builder of constraints for a specific member of a message.
    /// </summary>
    /// <typeparam name="T">Type of the object the error messages are produced for.</typeparam>
    public interface IMemberConstraintBuilder<in T> : IErrorMessageWriter<T>
    {
        /// <summary>
        /// The member the constraint is applied to.
        /// </summary>
        IMember Member
        {
            get;
        }        
    }
}

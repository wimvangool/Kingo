namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint for a specific member of a message.
    /// </summary>
    public interface IMemberConstraint : IErrorMessageProducer
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

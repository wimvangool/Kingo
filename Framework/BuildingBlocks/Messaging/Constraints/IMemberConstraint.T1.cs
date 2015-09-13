using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint for a specific member of a message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the object the error messages are produced for.</typeparam>
    public interface IMemberConstraint<in TMessage> : IErrorMessageProducer<TMessage>
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

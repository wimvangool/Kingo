using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Attribute that can be used to mark a member of a <see cref="DomainEvent{T, S, K}" /> as a member
    /// of a certain aggregate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class AggregateMemberAttribute : Attribute
    {
        /// <summary>
        /// A value indicating which type of member this field or property is referring to.
        /// </summary>
        public readonly AggregateMemberType MemberType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateMemberAttribute" /> class.
        /// </summary>
        /// <param name="memberType">A value indicating which type of member this field or property is referring to.</param>
        public AggregateMemberAttribute(AggregateMemberType memberType)
        {
            MemberType = memberType;
        }        
    }
}

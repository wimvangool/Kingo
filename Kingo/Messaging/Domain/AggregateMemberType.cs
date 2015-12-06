namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// A value indicating which type of member of an aggregate a field or property is referring to.
    /// </summary>
    public enum AggregateMemberType
    {
        /// <summary>
        /// Marks the member as no aggregate member.
        /// </summary>
        None = 0,

        /// <summary>
        /// Marks the member as the key of an aggregate.
        /// </summary>
        Key = 1,

        /// <summary>
        /// Marks the member as the version of an aggregate.
        /// </summary>
        Version = 2
    }
}

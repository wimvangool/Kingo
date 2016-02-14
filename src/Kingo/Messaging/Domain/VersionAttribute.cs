using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Can be used to decorate mark a field or property as the <see cref="IHasKeyAndVersion{T, K}.Version">Version</see> of an aggregate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class VersionAttribute : Attribute { }
}

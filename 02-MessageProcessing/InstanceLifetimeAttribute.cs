using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// This attribute can be used to control the lifetime of a dependency, which is ultimately managed
    /// by the Dependency Injection Framework of choice.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InstanceLifetimeAttribute : Attribute
    {
        private readonly InstanceLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceLifetimeAttribute" /> class.
        /// </summary>
        /// <param name="lifetime">The configured dependency lifetime.</param>
        public InstanceLifetimeAttribute(InstanceLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        /// <summary>
        /// The configured dependency lifetime.
        /// </summary>
        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }
    }
}

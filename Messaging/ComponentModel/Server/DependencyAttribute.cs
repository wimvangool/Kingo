namespace System.ComponentModel.Server
{
    /// <summary>
    /// This attribute must be put on each dependency class to support auto-registration of it
    /// by the <see cref="MessageHandlerFactory" /> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DependencyAttribute : Attribute, IDependencyConfiguration
    {
        private readonly InstanceLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyAttribute" /> class.
        /// </summary>
        /// <param name="lifetime">The lifetime of the dependency.</param>
        public DependencyAttribute(InstanceLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        /// <summary>
        /// The lifetime of the dependency.
        /// </summary>
        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }
    }
}

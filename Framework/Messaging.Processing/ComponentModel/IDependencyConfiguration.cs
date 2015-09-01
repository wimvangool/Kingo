using ServiceComponents.ComponentModel.Server;

namespace ServiceComponents.ComponentModel
{
    /// <summary>
    /// When implemented by a class, contains all configuration settings for a dependency.
    /// </summary>
    public interface IDependencyConfiguration
    {
        /// <summary>
        /// The lifetime of the dependency.
        /// </summary>
        InstanceLifetime Lifetime
        {
            get;
        }
    }
}

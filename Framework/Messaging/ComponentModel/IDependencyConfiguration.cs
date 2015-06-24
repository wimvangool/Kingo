using System.ComponentModel.Server;

namespace System.ComponentModel
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

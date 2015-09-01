using System;
using System.Collections.Generic;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    /// <summary>
    /// Represents a mapping of specific dependencies to
    /// their <see cref="IDependencyConfiguration">configuration settings</see>.
    /// </summary>
    public sealed class DependencyToConfigurationMapping : Dictionary<Type, IDependencyConfiguration>
    {
        private readonly IDependencyConfiguration _defaultConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyToConfigurationMapping" /> class.
        /// </summary>
        /// <param name="defaultConfiguration">
        /// The configuration to apply for those types that are not mapped to their own specific configuration.
        /// </param>
        public DependencyToConfigurationMapping(IDependencyConfiguration defaultConfiguration = null)
        {
            _defaultConfiguration = defaultConfiguration ?? DependencyConfiguration.Default;
        }

        /// <summary>
        /// The configuration to apply for those types that are not mapped to their own specific configuration.
        /// </summary>
        public IDependencyConfiguration DefaultConfiguration
        {
            get { return _defaultConfiguration; }
        }
    }
}

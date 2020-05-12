using System;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a set of options that can be set to configure the behavior of a <see cref="MicroProcessor" />.
    /// </summary>
    public sealed class MicroProcessorSettings
    {
        private MicroProcessorSettings(string serviceName)
        {
            ServiceName = serviceName;
            UnitOfWorkMode = UnitOfWorkMode.MultiThreaded;
        }

        private MicroProcessorSettings(MicroProcessorSettings settings)
        {
            ServiceName = settings.ServiceName;
            UnitOfWorkMode = settings.UnitOfWorkMode;
        }

        internal MicroProcessorSettings Copy() =>
            new MicroProcessorSettings(this);

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(ServiceName)} = {ServiceName}, {nameof(UnitOfWorkMode)} = {UnitOfWorkMode}";

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="UnitOfWorkMode" />, which determines how the processor should
        /// flush its unit of work managed by different <see cref="IChangeTracker">Change Trackers</see>.
        /// </summary>
        public UnitOfWorkMode UnitOfWorkMode
        {
            get;
            set;
        }

        internal static MicroProcessorSettings DefaultSettings() =>
            new MicroProcessorSettings(ResolveDefaultServiceName());

        private static string ResolveDefaultServiceName()
        {
            // If possible, the default name of the service is derived from the namespace of the
            // class where the main-method lives. If this is not possible, we fall back on the
            // name of the local machine.
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return Environment.MachineName;
            }
            return ResolveDefaultServiceName(assembly.EntryPoint.DeclaringType);
        }

        private static string ResolveDefaultServiceName(Type startupType) =>
            ResolveDefaultServiceName(startupType.FriendlyName(true, false));

        private static string ResolveDefaultServiceName(string startupTypeName)
        {
            // By convention, we assume the name of the service is the second segment
            // of the namespace (e.g. MyCompany.MyService.Startup).
            var namespaceSegments = startupTypeName.Split('.');
            if (namespaceSegments.Length == 1)
            {
                return startupTypeName;
            }
            return namespaceSegments[1];
        }
    }
}

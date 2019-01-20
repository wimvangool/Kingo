using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo
{
    internal sealed class ServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        private ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType) =>
            _serviceProvider.GetService(serviceType);

        private static readonly Lazy<ServiceProvider> _DefaultProvider = new Lazy<ServiceProvider>(CreateDefaultProvider, true);

        public static ServiceProvider Default =>
            _DefaultProvider.Value;

        private static ServiceProvider CreateDefaultProvider() =>
            new ServiceProvider(new ServiceCollection().BuildServiceProvider());
    }
}

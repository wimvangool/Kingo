using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroProcessorConfiguration : MicroProcessorConfigurationBase
    {
        #region [====== NotConfiguredState ======]

        private sealed class NotConfiguredState : MicroProcessorConfigurationBase
        {
            private readonly MicroProcessorConfiguration _serviceCollection;

            public NotConfiguredState(MicroProcessorConfiguration serviceCollection)
            {
                _serviceCollection = serviceCollection;
            }

            public override IServiceCollectionConfiguration Setup<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
                SetupProcessor<TProcessor>(processorConfigurator);

            private MicroProcessorConfigurationBase SetupProcessor<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null)
                where TProcessor : MicroProcessor
            {
                if (_serviceCollection.SwitchToState(this, new ConfiguringState<TProcessor>(_serviceCollection, processorConfigurator)))
                {
                    return _serviceCollection._state;
                }
                throw NewProcessorAlreadyConfiguredException();
            }

            public override void Configure(Action<IServiceCollection> serviceConfigurator) =>
                throw NewProcessorNotYetConfiguredException();

            public override IServiceProvider ServiceProvider() =>
                SetupProcessor<MicroProcessor>().ServiceProvider();
        }

        #endregion

        #region [====== ConfiguringState ======]

        private sealed class ConfiguringState<TProcessor> : MicroProcessorConfigurationBase
            where TProcessor : MicroProcessor
        {
            private readonly MicroProcessorConfiguration _serviceCollection;
            private readonly Action<IMicroProcessorBuilder> _processorConfigurator;
            private readonly Action<IServiceCollection> _serviceConfigurator;
            private readonly Lazy<IServiceProvider> _serviceProvider;

            public ConfiguringState(MicroProcessorConfiguration serviceCollection, Action<IMicroProcessorBuilder> processorConfigurator, Action<IServiceCollection> serviceConfigurator = null)
            {
                _serviceCollection = serviceCollection;
                _processorConfigurator = processorConfigurator;
                _serviceConfigurator = serviceConfigurator;
                _serviceProvider = new Lazy<IServiceProvider>(BuildServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);
            }

            public override IServiceCollectionConfiguration Setup<T>(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
                throw NewProcessorAlreadyConfiguredException();

            public override void Configure(Action<IServiceCollection> serviceConfigurator)
            {
                if (_serviceConfigurator == null)
                {
                    if (serviceConfigurator == null)
                    {
                        throw new ArgumentNullException(nameof(serviceConfigurator));
                    }
                    if (_serviceCollection.SwitchToState(this, new ConfiguringState<TProcessor>(_serviceCollection, _processorConfigurator, serviceConfigurator)))
                    {
                        return;
                    }
                }
                throw NewServicesAlreadyConfiguredException();
            }

            public override IServiceProvider ServiceProvider()
            {
                if (_serviceCollection.SwitchToState(this, new ConfiguredState(_serviceProvider.Value)))
                {
                    return _serviceCollection._state.ServiceProvider();
                }
                return _serviceProvider.Value;
            }

            private IServiceProvider BuildServiceProvider()
            {
                var services = new ServiceCollection();

                services.AddMicroProcessor<TProcessor>(processor =>
                {                    
                    _processorConfigurator?.Invoke(processor);
                });

                _serviceConfigurator?.Invoke(services);

                return services.BuildServiceProvider(true);
            }
        }

        #endregion

        #region [====== ConfiguredState ======]

        private sealed class ConfiguredState : MicroProcessorConfigurationBase            
        {
            private readonly IServiceProvider _serviceProvider;

            public ConfiguredState(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public override IServiceCollectionConfiguration Setup<T>(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
                throw NewProcessorAlreadyConfiguredException();

            public override void Configure(Action<IServiceCollection> serviceConfigurator) =>
                throw NewServicesAlreadyConfiguredException();

            public override IServiceProvider ServiceProvider() =>
                _serviceProvider;
        }

        #endregion

        private MicroProcessorConfigurationBase _state;

        public MicroProcessorConfiguration()
        {
            _state = new NotConfiguredState(this);
        }

        public override IServiceCollectionConfiguration Setup<TProcessor>(Action<IMicroProcessorBuilder> processorConfigurator = null) =>
            _state.Setup<TProcessor>(processorConfigurator);

        public override void Configure(Action<IServiceCollection> serviceConfigurator) =>
            _state.Configure(serviceConfigurator);

        public override IServiceProvider ServiceProvider() =>
            _state.ServiceProvider();

        // This method switches from one state to another in a thread-safe manner. It returns true if the switch has been made.
        private bool SwitchToState(MicroProcessorConfigurationBase oldState, MicroProcessorConfigurationBase newState) =>
            Interlocked.CompareExchange(ref _state, newState, oldState) == oldState;

        private static Exception NewProcessorAlreadyConfiguredException([CallerMemberName] string methodName = null)
        {
            var messageFormat = ExceptionMessages.MicroProcessorConfiguration_ProcessorAlreadyConfigured;
            var message = string.Format(messageFormat, methodName);
            return new InvalidOperationException(message);
        }

        private static Exception NewProcessorNotYetConfiguredException([CallerMemberName] string methodName = null)
        {
            var messageFormat = ExceptionMessages.MicroProcessorConfiguration_ProcessorNotYetConfigured;
            var message = string.Format(messageFormat, methodName);
            return new InvalidOperationException(message);
        }

        private static Exception NewServicesAlreadyConfiguredException([CallerMemberName] string methodName = null)
        {
            var messageFormat = ExceptionMessages.MicroProcessorConfiguration_ServicesAlreadyConfigured;
            var message = string.Format(messageFormat, methodName);
            return new InvalidOperationException(message);
        }
    }
}

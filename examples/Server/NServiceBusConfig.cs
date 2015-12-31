using System.ServiceModel;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Players;
using NServiceBus;
using NServiceBus.Config;

namespace Kingo.Samples.Chess
{    
    public sealed class NServiceBusConfig : IConfigureThisEndpoint,
                                            IWantToRunWhenConfigurationIsComplete,
                                            IWantToRunWhenBusStartsAndStops
    {        
        void IConfigureThisEndpoint.Customize(BusConfiguration configuration)
        {            
            configuration.AssembliesToScan(GetAssembliesToScan("*Chess.Api.dll", "*Chess.dll"));
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.UseContainer<UnityBuilder>();
            configuration.UseSerialization<JsonSerializer>();
        }
        
        void IWantToRunWhenConfigurationIsComplete.Run(Configure config)
        {
            _PlayerServiceHost = new ServiceHost(new PlayerService(CreateEnterpriseServiceBus(config)));
            _ChallengeServiceHost = new ServiceHost(new ChallengeService(CreateEnterpriseServiceBus(config)));
        }

        void IWantToRunWhenBusStartsAndStops.Start()
        {
            _PlayerServiceHost.Open();
            _ChallengeServiceHost.Open();
        }

        void IWantToRunWhenBusStartsAndStops.Stop()
        {
            _ChallengeServiceHost.Close();
            _PlayerServiceHost.Close();
        }

        private static ServiceHost _PlayerServiceHost;
        private static ServiceHost _ChallengeServiceHost;
        
        private static IBus CreateEnterpriseServiceBus(Configure config)
        {
            return config.Builder.Build<IBus>();
        }

        private static AssemblySet GetAssembliesToScan(params string[] searchPatterns)
        {
            return AssemblySet.FromCurrentDirectory(searchPatterns);
        }
    }
}

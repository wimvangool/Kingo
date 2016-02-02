using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Games;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess
{
    internal sealed class ServiceHostManager : IDisposable
    {
        private readonly ServiceHost[] _serviceHosts;
        private bool _isDisposed;

        private ServiceHostManager(ServiceHost[] serviceHosts)
        {
            _serviceHosts = serviceHosts;
        }

        internal void Open()
        {
            foreach (var serviceHost in _serviceHosts)
            {
                serviceHost.Open();
            }
        }        

        internal void Close()
        {
            foreach (var serviceHost in _serviceHosts.Reverse())
            {                
                serviceHost.Close();
            }
        }        

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            foreach (var disposable in Disposables())
            {
                disposable.Dispose();
            }
            _isDisposed = true;
        }

        private IEnumerable<IDisposable> Disposables()
        {
            return _serviceHosts.Reverse();
        }

        internal static ServiceHostManager CreateServiceHostManager()
        {
            var serviceHosts = CreateServiceHosts().ToArray();

            foreach (var serviceHost in serviceHosts)
            {
                serviceHost.Opening += (s, e) => HandleServiceHostOpening((ServiceHost) s);
                serviceHost.Opened += (s, e) => HandleServiceHostOpened((ServiceHost) s);
                serviceHost.Closing += (s, e) => HandleServiceHostClosing((ServiceHost) s);
                serviceHost.Closed += (s, e) => HandleServiceHostClosed((ServiceHost) s);
            }
            return new ServiceHostManager(serviceHosts);
        }

        private static IEnumerable<ServiceHost> CreateServiceHosts()
        {
            yield return new ServiceHost(typeof(PlayerService));
            yield return new ServiceHost(typeof(ChallengeService));
            yield return new ServiceHost(typeof(GameService));
        }

        private static void HandleServiceHostOpening(ServiceHost host)
        {
            Console.WriteLine("Starting service '{0}...'", host.Description.ServiceType.Name);
        }

        private static void HandleServiceHostOpened(ServiceHost host)
        {
            Console.WriteLine("Service up and running at:");

            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine("\t{0}", endpoint.Address);
            }
        }

        private static void HandleServiceHostClosing(ServiceHost host)
        {
            Console.WriteLine("Shutting down service '{0}'...", host.Description.ServiceType.Name);
        }

        private static void HandleServiceHostClosed(ServiceHost host) { }
    }
}

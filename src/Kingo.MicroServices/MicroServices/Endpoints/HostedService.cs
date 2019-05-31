using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    public abstract class HostedService : IHostedService
    {


        public Task StartAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();



        public Task StopAsync(CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}

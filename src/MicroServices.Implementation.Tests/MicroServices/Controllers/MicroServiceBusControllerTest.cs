using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MicroServiceBusControllerTest
    {
        #region [====== Stubs ======]

        private sealed class MicroServiceBusControllerStub : MicroServiceBusController
        {
            private readonly MicroServiceBusControllerOptions _options;

            public MicroServiceBusControllerStub(IMicroProcessor processor, MicroServiceBusModes modes) : base(processor)
            {
                _options = new MicroServiceBusControllerOptions()
                {
                    Modes = modes
                };
            }

            protected override MicroServiceBusControllerOptions Options =>
                _options;

            protected override MicroServiceBusClient CreateClient(IEnumerable<IMicroServiceBusEndpoint> endpoints) =>
                new MicroServiceBusClientStub(endpoints);
        }

        private sealed class MicroServiceBusClientStub : MicroServiceBusClient
        {
            public MicroServiceBusClientStub(IEnumerable<IMicroServiceBusEndpoint> endpoints) :
                base(endpoints) { }

            protected override Task<MicroServiceBusProxy> StartSenderAsync(CancellationToken token) =>
                throw new NotImplementedException();

            protected override Task<MicroServiceBusProxy> StartReceiverAsync(CancellationToken token) =>
                throw new NotImplementedException();
        }

        #endregion

        
    }
}

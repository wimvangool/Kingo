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

            protected override MicroServiceBus CreateServiceBus(IEnumerable<IMicroServiceBusEndpoint> endpoints) =>
                new MicroServiceBusClientStub(endpoints);
        }

        private sealed class MicroServiceBusClientStub : MicroServiceBus
        {
            public MicroServiceBusClientStub(IEnumerable<IMicroServiceBusEndpoint> endpoints) :
                base(endpoints) { }

            protected override Task<MicroServiceBusClient> CreateSenderAsync(CancellationToken token) =>
                throw new NotImplementedException();

            protected override Task<MicroServiceBusClient> CreateReceiverAsync(CancellationToken token) =>
                throw new NotImplementedException();
        }

        #endregion

        
    }
}

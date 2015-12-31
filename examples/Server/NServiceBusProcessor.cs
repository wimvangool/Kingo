using System;
using NServiceBus;

namespace Kingo.Samples.Chess
{
    internal sealed class NServiceBusProcessor : ServerProcessor
    {
        private readonly IBus _enterpriseServiceBus;

        internal NServiceBusProcessor(IBus enterpriseServiceBus)
        {
            if (enterpriseServiceBus == null)
            {
                throw new ArgumentNullException("enterpriseServiceBus");
            }
            _enterpriseServiceBus = enterpriseServiceBus;
        }

        protected override IBus EnterpriseServiceBus
        {
            get { return _enterpriseServiceBus; }
        }
    }
}

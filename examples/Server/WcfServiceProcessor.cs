using System.Collections.Generic;
using Kingo.Messaging;
using Kingo.Transactions;
using NServiceBus;

namespace Kingo.Samples.Chess
{
    internal sealed class WcfServiceProcessor : ServerProcessor
    {
        private readonly IBus _enterpriseServiceBus;

        internal WcfServiceProcessor(IBus enterpriseServiceBus)
        {
            _enterpriseServiceBus = enterpriseServiceBus;
        }

        protected override IBus EnterpriseServiceBus
        {
            get { return _enterpriseServiceBus; }
        }

        #region [====== Pipeline Configuration ======]

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            yield return new FaultExceptionModule();
            yield return new MessageValidationModule();
            yield return new TransactionScopeModule();
        }

        #endregion

             
    }
}

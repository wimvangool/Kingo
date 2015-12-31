using System;
using Kingo.Messaging;
using NServiceBus;

namespace Kingo.Samples.Chess
{
    public abstract class WcfService
    {
        private readonly Lazy<WcfServiceProcessor> _processor;

        protected WcfService()
        {
            _processor = new Lazy<WcfServiceProcessor>(() => new WcfServiceProcessor(EnterpriseServiceBus));
        }

        protected IMessageProcessor Processor
        {
            get { return _processor.Value; }
        }

        protected abstract IBus EnterpriseServiceBus
        {
            get;
        }
    }
}

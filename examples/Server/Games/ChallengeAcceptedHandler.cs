using Kingo.Samples.Chess.Challenges;
using NServiceBus;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ChallengeAcceptedHandler : IHandleMessages<ChallengeAcceptedEvent>
    {
        private readonly NServiceBusProcessor _processor;

        public ChallengeAcceptedHandler(IBus enterpriseServiceBus)
        {
            _processor = new NServiceBusProcessor(enterpriseServiceBus);
        }

        public void Handle(ChallengeAcceptedEvent message)
        {            
            _processor.Handle(message);
        }
    }
}

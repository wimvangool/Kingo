using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges;
using NServiceBus;

namespace Kingo.Samples.Chess.Games
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class GameService : WcfService, IGameService, IHandleMessages<ChallengeAcceptedEvent>
    {
        private readonly IBus _enterpriseServiceBus;
        private readonly NServiceBusProcessor _eventProcessor;
 
        public GameService(IBus enterpriseServiceBus)
        {
            if (enterpriseServiceBus == null)
            {
                throw new ArgumentNullException("enterpriseServiceBus");
            }
            _enterpriseServiceBus = enterpriseServiceBus;
            _eventProcessor = new NServiceBusProcessor(enterpriseServiceBus);
        }

        protected override IBus EnterpriseServiceBus
        {
            get { return _enterpriseServiceBus; }
        }

        #region [====== Events ======]

        void IHandleMessages<ChallengeAcceptedEvent>.Handle(ChallengeAcceptedEvent message)
        {
            _eventProcessor.Handle(message);
        }

        #endregion

        #region [====== Write Methods ======]

        public Task ForfeitGameAsync(ForfeitGameCommand command)
        {
            return Processor.HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetActiveGamesResponse> GetActiveGames(GetActiveGamesRequest request)
        {
            return Processor.ExecuteAsync(ActiveGamesTable.SelectByPlayerAsync, request);
        }

        #endregion
    }
}

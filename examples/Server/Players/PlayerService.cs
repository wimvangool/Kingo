using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges;
using NServiceBus;

namespace Kingo.Samples.Chess.Players
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class PlayerService : WcfService, IPlayerService
    {
        private readonly IBus _enterpriseServiceBus;

        public PlayerService(IBus enterpriseServiceBus)
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

        #region [====== Write Methods ======]

        /// <inheritdoc />
        public Task RegisterPlayerAsync(RegisterPlayerCommand command)
        {
            return Processor.HandleAsync(command);
        }

        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return Processor.HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        /// <inheritdoc />
        public Task<GetPlayersResponse> GetPlayersAsync(GetPlayersRequest request)
        {
            return Processor.ExecuteAsync(PlayersTable.SelectAllAsync, request);
        }       

        #endregion
    }
}

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using NServiceBus;

namespace Kingo.Samples.Chess.Challenges
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public sealed class ChallengeService : WcfService, IChallengeService
    {
        private readonly IBus _enterpriseServiceBus;

        public ChallengeService(IBus enterpriseServiceBus)
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

        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return Processor.HandleAsync(command);
        }

        public Task AcceptChallengeAsync(AcceptChallengeCommand command)
        {
            return Processor.HandleAsync(command);
        }

        public Task RejectChallengeAsync(RejectChallengeCommand command)
        {
            return Processor.HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetPendingChallengesResponse> GetPendingChallenges(GetPendingChallengesRequest request)
        {
            return Processor.ExecuteAsync(request, new GetPendingChallengesQuery());
        }

        #endregion        
    }
}

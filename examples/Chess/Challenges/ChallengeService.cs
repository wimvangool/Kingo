using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Challenges
{    
    public sealed class ChallengeService : ServiceProcessor, IChallengeService
    {                
        #region [====== Write Methods ======]

        public Task ChallengePlayerAsync(ChallengeUserCommand command)
        {
            return HandleAsync(command);
        }

        public Task AcceptChallengeAsync(AcceptChallengeCommand command)
        {
            return HandleAsync(command);
        }

        public Task RejectChallengeAsync(RejectChallengeCommand command)
        {
            return HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetPendingChallengesResponse> GetPendingChallenges(GetPendingChallengesRequest request)
        {
            return ExecuteAsync(PendingChallengesTable.SelectByReceiverAsync, request);
        }

        #endregion                  
    }
}

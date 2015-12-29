using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengeService : WcfServiceProcessor, IChallengeService
    {
        #region [====== Write Methods ======]

        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return HandleAsync(command);
        }

        public Task AcceptChallengeAsync(AcceptChallengeCommand command)
        {
            return HandleAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetPendingChallengesResponse> GetPendingChallenges(GetPendingChallengesRequest request)
        {
            return ExecuteAsync(request, new GetPendingChallengesQuery());
        }

        #endregion
    }
}

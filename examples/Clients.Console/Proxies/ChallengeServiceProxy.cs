using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges;

namespace Clients.ConsoleApp.Proxies
{
    internal sealed class ChallengeServiceProxy : ClientBase<IChallengeService>, IChallengeService
    {
        internal ChallengeServiceProxy()
            : base("ChallengeService_NetTcp") { }

        #region [====== Write Methods ======]

        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return Channel.ChallengePlayerAsync(command);
        }

        public Task AcceptChallengeAsync(AcceptChallengeCommand command)
        {
            return Channel.AcceptChallengeAsync(command);
        }

        public Task RejectChallengeAsync(RejectChallengeCommand command)
        {
            return Channel.RejectChallengeAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetPendingChallengesResponse> GetPendingChallenges(GetPendingChallengesRequest request)
        {
            return Channel.GetPendingChallenges(request);
        }

        #endregion       
    }
}

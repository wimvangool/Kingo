using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.ChessApplication.Challenges
{
    internal sealed class ChallengeServiceProxy : ClientBase<IChallengeService>, IChallengeService
    {
        public Task Execute(AcceptChallengeCommand command)
        {
            return Channel.Execute(command);
        }

        public Task Execute(RejectChallengeCommand command)
        {
            return Channel.Execute(command);
        }
    }
}

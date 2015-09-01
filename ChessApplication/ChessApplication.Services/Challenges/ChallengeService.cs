using System.Threading.Tasks;

namespace Kingo.ChessApplication.Challenges
{
    public sealed class ChallengeService : ChessApplicationService, IChallengeService
    {
        public Task Execute(AcceptChallengeCommand command)
        {
            return HandleAsync(command);
        }

        public Task Execute(RejectChallengeCommand command)
        {
            return HandleAsync(command);
        }
    }
}

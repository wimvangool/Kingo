using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Challenges
{
    public sealed class ChallengeService : WcfServiceProcessor, IChallengeService
    {
        public Task ChallengePlayerAsync(ChallengePlayerCommand command)
        {
            return HandleAsync(command);
        }
    }
}

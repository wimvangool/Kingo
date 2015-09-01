using System.Threading.Tasks;

namespace Kingo.ChessApplication.Players
{
    public sealed class PlayerService : ChessApplicationService, IPlayerService
    {
        public Task Execute(RegisterPlayerCommand command)
        {
            return HandleAsync(command);
        }

        public Task Execute(ChallengePlayerCommand command)
        {
            return HandleAsync(command);
        }
    }
}

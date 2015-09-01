using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.ChessApplication.Players
{
    internal sealed class PlayerServiceProxy : ClientBase<IPlayerService>, IPlayerService
    {
        public Task Execute(RegisterPlayerCommand command)
        {
            return Channel.Execute(command);
        }

        public Task Execute(ChallengePlayerCommand command)
        {
            return Channel.Execute(command);
        }
    }
}

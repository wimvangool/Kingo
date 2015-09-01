using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Players
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

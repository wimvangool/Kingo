using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Players
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Challenges
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Challenges
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

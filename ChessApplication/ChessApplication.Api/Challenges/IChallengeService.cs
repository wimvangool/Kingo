using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Challenges
{
    [ServiceContract]
    public interface IChallengeService
    {
        [OperationContract(Name = "AcceptChallenge")]
        Task Execute(AcceptChallengeCommand command);

        [OperationContract(Name = "RejectChallenge")]
        Task Execute(RejectChallengeCommand command);
    }
}

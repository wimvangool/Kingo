using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.ChessApplication.Challenges
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

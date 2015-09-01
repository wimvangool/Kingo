using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.ChessApplication.Players
{
    [ServiceContract]
    public interface IPlayerService
    {
        [OperationContract(Name = "RegisterPlayer")]
        Task Execute(RegisterPlayerCommand command);

        [OperationContract(Name = "ChallengePlayer")]
        Task Execute(ChallengePlayerCommand command);
    }
}

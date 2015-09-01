using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ChessApplication.Players
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

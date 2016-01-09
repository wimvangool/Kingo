using System.ServiceModel;
using System.Threading.Tasks;

namespace Kingo.Samples.Chess.Games
{
    [ServiceContract]
    public interface IGameService
    {
        #region [====== Write Methods ======]

        [OperationContract]
        Task ForfeitGameAsync(ForfeitGameCommand command);

        #endregion

        #region [====== Read Methods ======]

        [OperationContract]
        Task<GetActiveGamesResponse> GetActiveGames(GetActiveGamesRequest request);

        #endregion
    }
}

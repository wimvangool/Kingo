using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Games;

namespace Clients.ConsoleApp.Proxies
{
    internal sealed class GameServiceProxy : ClientBase<IGameService>, IGameService
    {
        #region [====== Write Methods ======]

        public Task MovePiece(MovePieceCommand command)
        {
            return Channel.MovePiece(command);
        }

        public Task ForfeitGameAsync(ForfeitGameCommand command)
        {
            return Channel.ForfeitGameAsync(command);
        }

        #endregion

        #region [====== Read Methods ======]

        public Task<GetActiveGamesResponse> GetActiveGames(GetActiveGamesRequest request)
        {
            return Channel.GetActiveGames(request);
        }        

        #endregion
    }
}

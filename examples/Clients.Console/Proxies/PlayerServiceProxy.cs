using System.ServiceModel;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Players;

namespace Clients.ConsoleApp.Proxies
{
    internal sealed class PlayerServiceProxy : ClientBase<IPlayerService>, IPlayerService
    {
        public PlayerServiceProxy()
            : base("PlayerService_NetTcp") { }

        #region [====== Write Methods ======]

        public Task RegisterPlayerAsync(RegisterPlayerCommand command)
        {
            return Channel.RegisterPlayerAsync(command);
        }       

        #endregion

        #region [====== Read Methods ======]

        public Task<GetPlayersResponse> GetPlayersAsync(GetPlayersRequest request)
        {
            return Channel.GetPlayersAsync(request);
        }

        #endregion
    }
}

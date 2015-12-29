using System;
using System.Linq;
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

        internal bool TryGetRegisteredPlayer(string enteredName, out RegisteredPlayer player)
        {
            var response = GetPlayersAsync(new GetPlayersRequest()).Result;
            var matches =
                from registeredPlayer in response.Players
                where string.Compare(registeredPlayer.Name, enteredName, StringComparison.OrdinalIgnoreCase) == 0
                select registeredPlayer;

            return (player = matches.FirstOrDefault()) != null;
        }

        public Task<GetPlayersResponse> GetPlayersAsync(GetPlayersRequest request)
        {
            return Channel.GetPlayersAsync(request);
        }

        #endregion
    }
}

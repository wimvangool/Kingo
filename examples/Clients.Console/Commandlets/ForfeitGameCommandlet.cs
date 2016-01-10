using System.Collections.Generic;
using Clients.ConsoleApp.Proxies;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ForfeitGameCommandlet : Commandlet
    {
        private readonly GameServiceProxy _gameService;

        internal ForfeitGameCommandlet()
            : base("Forfeit-Game", "Forfeit the current game.")
        {
            _gameService = new GameServiceProxy();
        }

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute();
        }

        internal void Execute()
        {
            
        }
    }
}

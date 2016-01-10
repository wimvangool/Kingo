using System;
using System.Collections.Generic;
using System.Linq;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Players;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class GetPlayerCommandlet : Commandlet
    {
        private readonly PlayerServiceProxy _playerService;

        internal GetPlayerCommandlet()
            : base("Get-Player", "Show the list of all registered players.")
        {
            _playerService = new PlayerServiceProxy();
        }

        internal override void Execute(IReadOnlyList<string> arguments)
        {
            Execute();
        }   
     
        internal void Execute()
        {
            var response = _playerService.GetPlayersAsync(new GetPlayersRequest()).Result;

            Console.WriteLine();

            foreach (var player in response.Players.OrderBy(p => p.Name))
            {
                Console.WriteLine(player.Name);
            }
            Console.WriteLine();
        }
    }
}

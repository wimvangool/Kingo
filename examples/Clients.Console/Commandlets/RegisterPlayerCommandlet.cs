using System;
using System.Collections.Generic;
using Clients.ConsoleApp.Proxies;
using Kingo.Samples.Chess.Players;
using Kingo.Threading;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class RegisterPlayerCommandlet : Commandlet
    {
        private const string _PlayerArgument = "<player>";
        private readonly PlayerServiceProxy _playerService;

        internal RegisterPlayerCommandlet()
            : base("Register-Player", "Register a new player.")
        {
            _playerService = new PlayerServiceProxy();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, _PlayerArgument);
        }

        protected override IEnumerable<string> Arguments()
        {
            yield return _PlayerArgument;
        }

        internal override void Execute(IReadOnlyList<string> arguments)
        {            
            Execute(arguments[0]);            
        }

        internal void Execute(string playerName)
        {
            _playerService.RegisterPlayerAsync(new RegisterPlayerCommand(Guid.NewGuid(), playerName)).Await();

            using (ChessApplication.UseColor(ConsoleColor.Green))
            {
                Console.WriteLine("Player '{0}' was registered.", playerName);
            }
        }
    }
}

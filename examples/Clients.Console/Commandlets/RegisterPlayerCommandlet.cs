using System;
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

        public override void Execute(string[] args)
        {
            if (args.Length <= 1)
            {
                throw new MissingCommandArgumentException(_PlayerArgument);
            }
            if (args.Length > 2)
            {
                throw new UnknownCommandArgumentException(args[2]);
            }
            Execute(args[1]);            
        }

        private void Execute(string playerName)
        {
            _playerService.RegisterPlayerAsync(new RegisterPlayerCommand(Guid.NewGuid(), playerName)).Await();

            using (ChessApplication.UseColor(ConsoleColor.Green))
            {
                Console.WriteLine("Player '{0}' was registered.", playerName);
            }
        }
    }
}

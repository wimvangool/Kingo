using System;

namespace Clients.ConsoleApp.Commandlets
{
    internal sealed class ShowHelpCommandlet : Commandlet
    {
        private readonly ChessApplicationState _state;

        internal ShowHelpCommandlet(ChessApplicationState state)
            : base("Show-Help", "Show this help.")
        {
            _state = state;
        }

        public override void Execute(string[] args)
        {
            if (args.Length > 1)
            {
                throw new UnknownCommandArgumentException(args[1]);
            }
            Execute();
        }

        internal void Execute()
        {
            using (ChessApplication.UseColor(ConsoleColor.Magenta))
            {
                Console.WriteLine();
                Console.WriteLine("Please enter one of the following commands: ");

                foreach (var command in _state.SupportedCommandLets())
                {
                    Console.WriteLine("  {0}", command);
                    Console.WriteLine("    - {0}", command.Description);
                    Console.WriteLine();
                }
            }  
        }
    }
}

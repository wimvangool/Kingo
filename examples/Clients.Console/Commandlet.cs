using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Samples.Chess;

namespace Clients.ConsoleApp
{
    internal abstract class Commandlet
    {
        internal readonly string Name;
        internal readonly string Description;

        protected Commandlet(string name, string description)
        {
            Name = name;
            Description = description;
        }

        private bool IsMatch(string commandName)
        {
            return string.Compare(Name, commandName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override string ToString()
        {
            return Name;
        }        

        public abstract void Execute(string[] args);

        public static void ExecuteOneOf(IEnumerable<Commandlet> commands)
        {
            var command = NextCommand();
            var commandName = command[0];

            var commandToExecute = commands.FirstOrDefault(c => c.IsMatch(commandName));
            if (commandToExecute == null)
            {
                throw new UnknownCommandException(commandName);
            }
            commandToExecute.Execute(command);
        }        

        private static string[] NextCommand()
        {
            var session = Session.Current;
            var playerName = session == null ? null : session.PlayerName;
            string command;

            do
            {
                Console.Write(playerName);
                Console.Write('>');

                command = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(command));

            return command.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }        
    }
}

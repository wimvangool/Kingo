using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void ExecuteNextCommand(string commandPrompt, IEnumerable<Commandlet> commands)
        {
            var command = NextCommand(commandPrompt);
            var commandName = command[0];

            var commandToExecute = commands.FirstOrDefault(c => c.IsMatch(commandName));
            if (commandToExecute != null)
            {
                commandToExecute.Execute(command);
                return;
            }
            throw new UnknownCommandException(commandName);
        }        

        private static string[] NextCommand(string commandPrompt)
        {            
            string command;

            do
            {
                Console.Write(commandPrompt);
                Console.Write('>');

                command = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(command));

            return command.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }        
    }
}

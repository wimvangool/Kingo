using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Nito.AsyncEx;

namespace Kingo.ChessApplication.UserCommandExecution
{
    internal sealed class UserCommandProcessor : MessageProcessor
    {                
        private UserCommandProcessorState _state;

        private UserCommandProcessor()
        {            
            _state = new LoggedOutState(this);
        }

        private IUserCommand NextCommand()
        {
            _state.WaitForNextCommand();

            try
            {
                return UserCommand.Parse(Console.ReadLine());
            }
            catch (UserCommandParseException exception)
            {
                WriteError(exception.Message, exception.ErrorMessages);
                return new NullCommand();
            }
        }

        private async Task<bool> ExecuteAsync(IUserCommand command)
        {
            try
            {
                return await command.ExecuteWithAsync(_state); 
            }
            catch (UserCommandParseException exception)
            {
                WriteError(exception.Message, exception.ErrorMessages);
                return true;
            }
            catch (UserCommandExecutionException exception)
            {
                WriteError(exception.Message);
                return true;
            }                                  
        }                      

        internal UserCommandProcessorState MoveTo(UserCommandProcessorState state)
        {
            return Interlocked.CompareExchange(ref _state, state, _state);
        }               

        private static void Main(string[] args)
        {
            AsyncContext.Run(() => Execute(Parse(args)));
        }

        private static IUserCommand Parse(IReadOnlyCollection<string> args)
        {
            //return args.Count == 0
            //    ? new NullCommand()
            //    : UserCommand.Parse("Login-Player", args);  
            return new NullCommand();
        }  

        private static async Task Execute(IUserCommand command)
        {            
            WriteInfo("Welcome to Chess Games!");

            var application = new UserCommandProcessor();

            try
            {
                while (await application.ExecuteAsync(command))
                {
                    command = application.NextCommand();
                } 
            }
            catch (Exception exception)
            {
                WriteError(exception.Message);
                Environment.ExitCode = 1;                
            }                                 
        }        

        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            try
            {
                Console.WriteLine(message);
                Console.WriteLine();
            }
            finally
            {
                Console.ResetColor();
            }
        }

        internal static void WriteError(string message, IEnumerable<string> details = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            try
            {
                Console.WriteLine(message);

                foreach (var detail in details ?? Enumerable.Empty<string>())
                {
                    Console.Write('\t');
                    Console.WriteLine(detail);
                }
                Console.WriteLine();
            }
            finally
            {
                Console.ResetColor();
            }
        }  
    }
}

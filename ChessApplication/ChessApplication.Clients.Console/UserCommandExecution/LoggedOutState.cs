using System;
using System.Text;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.ChessApplication.Players;

namespace Kingo.ChessApplication.UserCommandExecution
{
    internal sealed class LoggedOutState : UserCommandProcessorState,
                                           IMessageHandler<RegisterPlayerCommand>
    {
        private readonly IMessageProcessor _processor;
        private readonly IPlayerService _playerService;

        internal LoggedOutState(IMessageProcessor processor)
        {
            _processor = processor;  
            _playerService = new PlayerServiceProxy();

            Register("Register-Player", Parse, this);
        }

        protected override IMessageProcessor Processor
        {
            get { return _processor; }
        }        

        private RegisterPlayerCommand Parse(UserCommandArgumentStack arguments)
        {
            var username = arguments.PopString("Username");

            arguments.ThrowIfNotEmpty();

            WaitForInput(@"Enter Password (1st)");

            var password1 = ReadPassword();

            WaitForInput(@"Enter Password (2nd)");

            var password2 = ReadPassword();
            if (password2 == password1)
            {
                return new RegisterPlayerCommand(username, password1);
            }
            throw new UserCommandExecutionException("The entered passwords did not match.");
        }

        Task IMessageHandler<RegisterPlayerCommand>.HandleAsync(RegisterPlayerCommand message)
        {
            return _playerService.Execute(message);
        }

        private static string ReadPassword()
        {
            var password = new StringBuilder();
            const char characterMask = '*';
            char enteredCharacter;

            while (TryReadPasswordCharacter(out enteredCharacter))
            {
                password.Append(enteredCharacter);

                Console.Write(characterMask);
            }
            Console.WriteLine();

            return password.ToString();
        }

        private static bool TryReadPasswordCharacter(out char enteredCharacter)
        {
            var keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                enteredCharacter = '\0';
                return false;
            }
            enteredCharacter = keyInfo.KeyChar;
            return true;
        }
    }
}

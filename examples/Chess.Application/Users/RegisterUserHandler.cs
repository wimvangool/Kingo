using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Validation;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Users
{
    public sealed class RegisterUserHandler : IMessageHandler<RegisterUserCommand>
    {
        private readonly IUserRepository _players;
        private readonly IUserAdministration _playerAdministration;

        public RegisterUserHandler(IUserRepository players, IUserAdministration playerAdministration)
        {            
            _players = players;
            _playerAdministration = playerAdministration;
        }

        public async Task HandleAsync(RegisterUserCommand message, IMicroProcessorContext context)
        {            
            var playerName = Identifier.Parse(message.UserName);

            var playerNameHasBeenRegistered = await _playerAdministration.HasBeenRegisteredAsync(playerName);
            if (playerNameHasBeenRegistered)
            {
                throw NewPlayerNameAlreadyRegisteredException(playerName);
            }
            _players.AddAsync(User.Register(message.UserId, playerName));
        }

        private static Exception NewPlayerNameAlreadyRegisteredException(Identifier playerName)
        {            
            return DomainException.CreateException(ExceptionMessages.Players_PlayerNameAlreadyRegistered, playerName);
        }
    }
}

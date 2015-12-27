using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Players
{
    public sealed class RegisterPlayerHandler : MessageHandler<RegisterPlayerCommand>
    {
        private readonly IPlayerRepository _players;

        public RegisterPlayerHandler(IPlayerRepository players)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players");
            }
            _players = players;
        }

        public override async Task HandleAsync(RegisterPlayerCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var playerName = Identifier.Parse(message.PlayerName);

            var playerNameHasBeenRegistered = await _players.HasBeenRegisteredAsync(playerName);
            if (playerNameHasBeenRegistered)
            {
                throw NewPlayerNameAlreadyRegisteredException(playerName);
            }
            _players.Add(Player.Register(message.PlayerId, playerName));
        }

        private static Exception NewPlayerNameAlreadyRegisteredException(Identifier playerName)
        {            
            return DomainException.CreateException(DomainExceptionMessages.Players_PlayerNameAlreadyRegistered, playerName);
        }
    }
}

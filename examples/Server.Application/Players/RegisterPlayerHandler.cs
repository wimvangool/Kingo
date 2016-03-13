using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Players
{
    public sealed class RegisterPlayerHandler : MessageHandler<RegisterPlayerCommand>
    {
        private readonly IPlayerRepository _players;
        private readonly IPlayerAdministration _playerAdministration;

        public RegisterPlayerHandler([NotNull] IPlayerRepository players, [NotNull] IPlayerAdministration playerAdministration)
        {            
            _players = players;
            _playerAdministration = playerAdministration;
        }

        public override async Task HandleAsync([NotNull] RegisterPlayerCommand message)
        {            
            var playerName = Identifier.Parse(message.PlayerName);

            var playerNameHasBeenRegistered = await _playerAdministration.HasBeenRegisteredAsync(playerName);
            if (playerNameHasBeenRegistered)
            {
                throw NewPlayerNameAlreadyRegisteredException(playerName);
            }
            _players.Add(Player.Register(message.PlayerId, playerName));
        }

        private static Exception NewPlayerNameAlreadyRegisteredException(Identifier playerName)
        {            
            return DomainException.CreateException(ExceptionMessages.Players_PlayerNameAlreadyRegistered, playerName);
        }
    }
}

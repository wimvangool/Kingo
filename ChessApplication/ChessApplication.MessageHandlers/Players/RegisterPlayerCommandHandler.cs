using System;
using System.Threading.Tasks;
using ServiceComponents.ComponentModel.Server;
using ServiceComponents.ComponentModel.Server.Domain;
using ServiceComponents.Threading;

namespace ServiceComponents.ChessApplication.Players
{
    /// <summary>
    /// Handles the <see cref="RegisterPlayerCommand" />.
    /// </summary>	
    [MessageHandler(InstanceLifetime.PerResolve)]
    public sealed class RegisterPlayerCommandHandler : MessageHandler<RegisterPlayerCommand>
    {
        private readonly IPlayerRepository _players;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPlayerCommandHandler" /> class.
        /// </summary>
        /// <param name="players">A <see cref="IPlayerRepository" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="players" /> is <c>null</c>.
        /// </exception>
        public RegisterPlayerCommandHandler(IPlayerRepository players)
        {
            if (players == null)
            {
                throw new ArgumentNullException("players");
            }
            _players = players;
        }

        /// <inheritdoc />
        [Throws(typeof(InvalidUsernameException))]
        [Throws(typeof(InvalidPasswordException))]
        public override Task HandleAsync(RegisterPlayerCommand message)
        {
            return AsyncMethod.RunSynchronously(() => Handle(message));
        }

        private void Handle(RegisterPlayerCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var username = Username.Parse(message.Username);
            var password = Password.Parse(message.Password);            

            _players.Add(Player.Register(message.PlayerId, username, password));
        }
    }
}
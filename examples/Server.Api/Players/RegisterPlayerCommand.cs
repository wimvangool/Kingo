using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Resources;
using NServiceBus;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// Represents a request to register a new player.
    /// </summary>
    [DataContract]
    public sealed class RegisterPlayerCommand : Message, ICommand
    {
        /// <summary>
        /// Identifier of the player.
        /// </summary>
        [DataMember]
        public readonly Guid PlayerId;

        /// <summary>
        /// Name of the player.
        /// </summary>
        [DataMember]
        public readonly string PlayerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPlayerCommand" /> class.
        /// </summary>
        /// <param name="playerId">Identifier of the player.</param>
        /// <param name="playerName">Name of the player.</param>
        public RegisterPlayerCommand(Guid playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<RegisterPlayerCommand>();

            validator.VerifyThat(m => m.PlayerId).IsNotEmpty(ErrorMessages.Message_ValueNotSpecified);
            validator.VerifyThat(m => m.PlayerName).IsNotNull(ErrorMessages.Message_ValueNotSpecified);
            validator.VerifyThat(m => m.PlayerName).IsIdentifier(ErrorMessages.RegisterPlayerCommand_InvalidPlayerName);

            return validator;
        }
    }
}
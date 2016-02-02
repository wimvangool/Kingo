using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// This event is raised when a new player has been registered.
    /// </summary>
    [DataContract]
    public sealed class PlayerRegisteredEvent : DomainEvent
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
        /// Version of the player.
        /// </summary>
        [DataMember]
        public new readonly int Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRegisteredEvent" /> class.
        /// </summary>
        /// <param name="playerId">Identifier of the player.</param>
        /// <param name="version">Version of the player.</param>
        /// <param name="playerName">Name of the player.</param>
        public PlayerRegisteredEvent(Guid playerId, int version, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Version = version;
        }        

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<PlayerRegisteredEvent>();

            validator.VerifyThat(m => m.PlayerId).IsNotEmpty();
            validator.VerifyThat(m => m.PlayerName).IsNotNull().IsIdentifier();
            validator.VerifyThat(m => m.Version).IsEqualTo(1);

            return validator;
        }
    }
}

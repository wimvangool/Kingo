using System;
using ServiceComponents.ChessApplication.Resources;
using ServiceComponents;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Constraints;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Players
{
    /// <summary>
    /// Occurs when a player has been registered.
    /// </summary>
    public sealed class PlayerRegisteredEvent : Message<PlayerRegisteredEvent>, IVersionedObject<Guid, DateTimeOffset>
    {
        public readonly Guid PlayerId;
        public readonly DateTimeOffset PlayerVersion;
        public string Username;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRegisteredEvent" /> class.
        /// </summary>
        /// <param name="playerId">Identifier of a player.</param>
        /// <param name="playerVersion">Version of a player.</param>
        public PlayerRegisteredEvent(Guid playerId, DateTimeOffset playerVersion)
        {
            PlayerId = playerId;
            PlayerVersion = playerVersion;
        }

        #region [====== Copy ======]

        private PlayerRegisteredEvent(PlayerRegisteredEvent message)
            : base(message)
        {
            PlayerId = message.PlayerId;
            PlayerVersion = message.PlayerVersion;
            Username = message.Username;
        }

        /// <inheritdoc />
        public override PlayerRegisteredEvent Copy()
        {
            return new PlayerRegisteredEvent(this);
        }

        #endregion

        #region [====== Id & Version ======]

        Guid IKeyedObject<Guid>.Key
        {
            get { return PlayerId; }
        }

        DateTimeOffset IVersionedObject<Guid, DateTimeOffset>.Version
        {
            get { return PlayerVersion; }
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as PlayerRegisteredEvent);
        }

        /// <inheritdoc />
        public bool Equals(PlayerRegisteredEvent other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return
                PlayerId.Equals(other.PlayerId) &&
                PlayerVersion.Equals(other.PlayerVersion) &&
                Username == other.Username;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(PlayerId, PlayerVersion);
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => PlayerId).IsNotEmpty(ValidationErrorMessages.PlayerRegisteredEvent_PlayerId_NotSpecified);
            validator.VerifyThat(() => Username).IsNotNullOrWhiteSpace(ValidationErrorMessages.PlayerRegisteredEvent_Username_NotSpecified);

            return validator;
        }

        #endregion
    }
}
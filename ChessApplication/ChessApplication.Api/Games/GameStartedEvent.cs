using System;
using Kingo.BuildingBlocks;
using Kingo.BuildingBlocks.Constraints;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Domain;

namespace Kingo.ChessApplication.Games
{
    /// <summary>
    /// Occurs when a new game is started.
    /// </summary>
    public sealed class GameStartedEvent : Message<GameStartedEvent>, IVersionedObject<Guid, DateTimeOffset>
    {
        public readonly Guid GameId;
        public readonly DateTimeOffset Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStartedEvent" /> class.
        /// </summary>
        /// <param name="gameId">Identifier of a game.</param>
        /// <param name="version">Version of a game.</param>
        public GameStartedEvent(Guid gameId, DateTimeOffset version)
        {
            GameId = gameId;
            Version = version;
        }

        /// <summary>
        /// Gets or sets the identifier of the player that will be playing with white.
        /// </summary>
        public Guid WhitePlayerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier of the player that will be playing with black.
        /// </summary>
        public Guid BlackPlayerId
        {
            get;
            set;
        }

        #region [====== Copy ======]

        private GameStartedEvent(GameStartedEvent message)
            : base(message)
        {
            GameId = message.GameId;
            Version = message.Version;
            WhitePlayerId = message.WhitePlayerId;
            BlackPlayerId = message.BlackPlayerId;
        }

        /// <inheritdoc />
        public override GameStartedEvent Copy()
        {
            return new GameStartedEvent(this);
        }

        #endregion

        #region [====== Id & Version ======]

        Guid IKeyedObject<Guid>.Key
        {
            get { return GameId; }
        }

        DateTimeOffset IVersionedObject<Guid, DateTimeOffset>.Version
        {
            get { return Version; }
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as GameStartedEvent);
        }

        /// <inheritdoc />
        public bool Equals(GameStartedEvent other)
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
                GameId.Equals(other.GameId) &&
                Version.Equals(other.Version) &&
                WhitePlayerId == other.WhitePlayerId &&
                BlackPlayerId == other.BlackPlayerId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(GameId, Version);
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        protected override IMessageValidator<GameStartedEvent> CreateValidator()
        {
            var validator = new ConstraintValidator<GameStartedEvent>();

            validator.VerifyThat(m => m.GameId).IsNotEmpty();
            validator.VerifyThat(m => m.WhitePlayerId).IsNotEmpty();
            validator.VerifyThat(m => m.BlackPlayerId).IsNotEmpty();

            return validator;
        }

        #endregion
    }
}
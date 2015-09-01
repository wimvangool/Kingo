using System;
using ServiceComponents;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// Occurs when a player has challenged another player.
    /// </summary>
    public sealed class PlayerChallengedEvent : Message<PlayerChallengedEvent>, IVersionedObject<Guid, DateTimeOffset>
    {
        public readonly Guid ChallengeId;
        public readonly DateTimeOffset Version;
        public Guid SenderId;
        public Guid ReceiverId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerChallengedEvent" /> class.
        /// </summary>
        /// <param name="challengeId">Identifier of a challenge.</param>
        /// <param name="version">Version of a challenge.</param>
        public PlayerChallengedEvent(Guid challengeId, DateTimeOffset version)
        {
            ChallengeId = challengeId;
            Version = version;
        }

        #region [====== Copy ======]

        private PlayerChallengedEvent(PlayerChallengedEvent message)
            : base(message)
        {
            ChallengeId = message.ChallengeId;
            Version = message.Version;

            SenderId = message.SenderId;
            ReceiverId = message.ReceiverId;
        }

        /// <inheritdoc />
        public override PlayerChallengedEvent Copy()
        {
            return new PlayerChallengedEvent(this);
        }

        #endregion

        #region [====== Id & Version ======]

        Guid IKeyedObject<Guid>.Key
        {
            get { return ChallengeId; }
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
            return Equals(obj as PlayerChallengedEvent);
        }

        /// <inheritdoc />
        public bool Equals(PlayerChallengedEvent other)
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
                ChallengeId.Equals(other.ChallengeId) &&
                Version.Equals(other.Version) &&
                SenderId.Equals(other.SenderId) &&
                ReceiverId.Equals(other.ReceiverId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(ChallengeId, Version);
        }

        #endregion        
    }
}
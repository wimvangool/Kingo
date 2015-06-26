using System;
using System.ComponentModel;
using System.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Challenges
{
    /// <summary>
    /// Occurs when a challenge was accepted.
    /// </summary>
    public sealed class ChallengeAcceptedEvent : Message<ChallengeAcceptedEvent>,
        IVersionedObject<Guid, DateTimeOffset>
    {
        public readonly Guid ChallengeId;
        public readonly DateTimeOffset Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeAcceptedEvent" /> class.
        /// </summary>
        /// <param name="challengeId">Identifier of a challenge.</param>
        /// <param name="version">Version of a challenge.</param>
        public ChallengeAcceptedEvent(Guid challengeId, DateTimeOffset version)
        {
            ChallengeId = challengeId;
            Version = version;
        }

        #region [====== Copy ======]

        private ChallengeAcceptedEvent(ChallengeAcceptedEvent message)
            : base(message)
        {
            ChallengeId = message.ChallengeId;
            Version = message.Version;
        }

        /// <inheritdoc />
        public override ChallengeAcceptedEvent Copy()
        {
            return new ChallengeAcceptedEvent(this);
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
            return Equals(obj as ChallengeAcceptedEvent);
        }

        /// <inheritdoc />
        public bool Equals(ChallengeAcceptedEvent other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return ChallengeId.Equals(other.ChallengeId) && Version.Equals(other.Version);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(ChallengeId, Version);
        }

        #endregion

        #region [====== Validation ======]        

        #endregion
    }
}
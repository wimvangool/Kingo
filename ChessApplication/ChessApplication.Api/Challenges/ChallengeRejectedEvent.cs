using System;
using ServiceComponents;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Constraints;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// Occurs when a challenge has been rejected.
    /// </summary>
    public sealed class ChallengeRejectedEvent : Message<ChallengeRejectedEvent>, IVersionedObject<Guid, DateTimeOffset>
    {
        public readonly Guid ChallengeId;
        public readonly DateTimeOffset Version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeRejectedEvent" /> class.
        /// </summary>
        /// <param name="challengeId">Identifier of a challenge.</param>
        /// <param name="version">Version of a challenge.</param>
        public ChallengeRejectedEvent(Guid challengeId, DateTimeOffset version)
        {
            ChallengeId = challengeId;
            Version = version;
        }

        #region [====== Copy ======]

        private ChallengeRejectedEvent(ChallengeRejectedEvent message)
            : base(message)
        {
            ChallengeId = message.ChallengeId;
            Version = message.Version;
        }

        /// <inheritdoc />
        public override ChallengeRejectedEvent Copy()
        {
            return new ChallengeRejectedEvent(this);
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
            return Equals(obj as ChallengeRejectedEvent);
        }

        /// <inheritdoc />
        public bool Equals(ChallengeRejectedEvent other)
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

        /// <inheritdoc />
        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => ChallengeId).IsNotEmpty();

            return validator;
        }

        #endregion
    }
}
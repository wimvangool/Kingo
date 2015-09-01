using System;
using ServiceComponents;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Constraints;

namespace ServiceComponents.ChessApplication.Challenges
{
    /// <summary>
    /// Represents a request to accept a challenge.
    /// </summary>
    public sealed class AcceptChallengeCommand : Message<AcceptChallengeCommand>
    {
        public readonly Guid ChallengeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptChallengeCommand" /> class.
        /// </summary>
        /// <param name="challengeId">Identifier of a challenge.</param>
        public AcceptChallengeCommand(Guid challengeId)
        {
            ChallengeId = challengeId;
        }

        #region [====== Copy ======]

        private AcceptChallengeCommand(AcceptChallengeCommand message)
            : base(message)
        {
            ChallengeId = message.ChallengeId;
        }

        /// <inheritdoc />
        public override AcceptChallengeCommand Copy()
        {
            return new AcceptChallengeCommand(this);
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as AcceptChallengeCommand);
        }

        /// <inheritdoc />
        public bool Equals(AcceptChallengeCommand other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return ChallengeId.Equals(other.ChallengeId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(ChallengeId);
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        protected override IMessageValidator CreateValidator()
        {
            //throw new NotImplementedException();
            return new ConstraintValidator();
        }

        #endregion
    }
}
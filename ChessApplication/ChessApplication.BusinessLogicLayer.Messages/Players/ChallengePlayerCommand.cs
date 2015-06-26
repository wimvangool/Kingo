using System;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Represents a request to let one player challenge another player.
    /// </summary>
    public sealed class ChallengePlayerCommand : Message<ChallengePlayerCommand>
    {
        public readonly Guid ChallengeId;
        public readonly Guid SenderId;
        public readonly Guid ReceiverId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengePlayerCommand" /> class.
        /// </summary>
        /// <param name="senderId">Identifier of a player that sends the challenge.</param>
        /// <param name="receiverId">Identifier of a player that receives the challenge.</param>
        public ChallengePlayerCommand(Guid senderId, Guid receiverId)
        {
            ChallengeId = Guid.NewGuid();
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        #region [====== Copy ======]

        private ChallengePlayerCommand(ChallengePlayerCommand message)
            : base(message)
        {
            ChallengeId = message.ChallengeId;
            SenderId = message.SenderId;
            ReceiverId = message.ReceiverId;
        }

        /// <inheritdoc />
        public override ChallengePlayerCommand Copy()
        {
            return new ChallengePlayerCommand(this);
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ChallengePlayerCommand);
        }

        /// <inheritdoc />
        public bool Equals(ChallengePlayerCommand other)
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
                SenderId.Equals(other.SenderId) &&
                ReceiverId.Equals(other.ReceiverId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(ChallengeId, SenderId, ReceiverId);
        }

        #endregion

        #region [====== Validation ======]

        /// <inheritdoc />
        protected override IMessageValidator CreateValidator()
        {
            var validator = new FluentValidator();

            validator.VerifyThat(() => SenderId).IsNotEmpty();
            validator.VerifyThat(() => ReceiverId).IsNotEmpty();

            return validator;
        }

        #endregion
    }
}
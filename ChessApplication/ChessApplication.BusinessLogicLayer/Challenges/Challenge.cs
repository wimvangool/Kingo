using System;
using System.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Challenges
{
    /// <summary>
    /// Represents a challenge.
    /// </summary>
    public sealed class Challenge : AggregateRoot<Guid, DateTimeOffset>
    {
        private readonly Guid _id;
        private readonly Guid _senderId;
        private readonly Guid _receiverId;
        private DateTimeOffset _version;        

        private Challenge(Guid id, Guid senderId, Guid receiverId)
        {
            _id = id;
            _senderId = senderId;
            _receiverId = receiverId;
            _version = DateTimeOffset.MinValue;
        }        

        internal static Challenge CreateChallenge(Guid challengeId, Guid senderId, Guid receiverId)
        {
            var challenge = new Challenge(challengeId, senderId, receiverId);

            challenge.Publish((id, version) => new PlayerChallengedEvent(id, version)
            {
                SenderId = senderId,
                ReceiverId = receiverId
            });
            return challenge;
        }

        #region [====== Id & Version ======]

        /// <inheritdoc />
        public override Guid Id
        {
            get { return _id; }
        }

        /// <inheritdoc />
        protected override DateTimeOffset Version
        {
            get { return _version; }
            set { SetVersion(ref _version, value); }
        }

        /// <inheritdoc />
        protected override DateTimeOffset NewVersion()
        {
            return Timestamp();
        }

        private static DateTimeOffset Timestamp()
        {
            return Clock.Current.UtcDateAndTime();
        }

        #endregion

        internal void Accept()
        {
            Publish((id, version) => new ChallengeAcceptedEvent(id, version));
        }

        internal void Reject()
        {
            Publish((id, version) => new ChallengeRejectedEvent(id, version));
        }
    }
}
using System;
using SummerBreeze.ChessApplication.Games;
using Syztem;
using Syztem.ComponentModel.Server.Domain;

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
        private ChallengeState _state;

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
            return Clock.Current.UtcDateAndTime();
        }        

        #endregion

        internal void Accept()
        {
            if (_state == ChallengeState.New)
            {
                _state = ChallengeState.Accepted;

                Publish((id, version) => new ChallengeAcceptedEvent(id, version));
            }
            else if (_state == ChallengeState.Accepted)
            {
                throw new ChallengeAlreadyAcceptedException(Id);
            }
            else if (_state == ChallengeState.Rejected)
            {
                throw new ChallengeAlreadyRejectedException(Id);
            }
        }

        internal void Reject()
        {
            if (_state == ChallengeState.New)
            {
                _state = ChallengeState.Rejected;

                Publish((id, version) => new ChallengeRejectedEvent(id, version));
            }
            else if (_state == ChallengeState.Accepted)
            {
                throw new ChallengeAlreadyAcceptedException(Id);
            }
            else if (_state == ChallengeState.Rejected)
            {
                throw new ChallengeAlreadyRejectedException(Id);
            }
        }

        internal Game StartNewGame()
        {
            if (_state == ChallengeState.Accepted)
            {
                return Game.StartNewGame(_senderId, _receiverId);
            }
            throw new ChallengeNotAcceptedException(Id);
        }
    }
}
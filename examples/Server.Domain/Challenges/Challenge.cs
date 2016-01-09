using System;
using Kingo.Clocks;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Games;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Challenges
{
    [Serializable]
    public sealed class Challenge : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private readonly Guid _senderId;
        private readonly Guid _receiverId;

        private ChallengeState _state;
        private int _version;

        internal Challenge(PlayerChallengedEvent @event)
            : base(@event)
        {
            _id = @event.ChallengeId;
            _senderId = @event.SenderId;
            _receiverId = @event.ReceiverId;

            _state = ChallengeState.Pending;
            _version = @event.ChallengeVersion;
        }

        public override Guid Id
        {
            get { return _id; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public void Accept()
        {
            if (!_receiverId.Equals(Session.Current.PlayerId))
            {
                throw NewPlayerCannotAcceptChallengeException(Session.Current.PlayerName);                
            }
            if (_state == ChallengeState.Accepted)
            {
                throw NewChallengeAlreadyAcceptedException();
            }
            if (_state == ChallengeState.Rejected)
            {
                throw NewChallengeAlreadyRejectedException();
            }
            _state = ChallengeState.Accepted;

            Publish(new ChallengeAcceptedEvent(_id, NextVersion()));                
        }

        private static Exception NewPlayerCannotAcceptChallengeException(string playerName)
        {
            var messageFormat = DomainExceptionMessages.Challenges_PlayerCannotAcceptChallenge;
            var message = string.Format(messageFormat, playerName);
            return new DomainException(message);
        }

        public void Reject()
        {
            if (!_receiverId.Equals(Session.Current.PlayerId))
            {
                throw NewPlayerCannotAcceptChallengeException(Session.Current.PlayerName);
            }
            if (_state == ChallengeState.Accepted)
            {
                throw NewChallengeAlreadyAcceptedException();
            }
            if (_state == ChallengeState.Rejected)
            {
                throw NewChallengeAlreadyRejectedException();
            }
            _state = ChallengeState.Rejected;

            Publish(new ChallengeRejectedEvent(_id, NextVersion()));                
        }

        public Game StartGame()
        {
            return new Game(CreateGameStartedEvent());
        }

        private GameStartedEvent CreateGameStartedEvent()
        {
            // We leave the assignment of white and black up to chance.
            var milliseconds = Clock.Current.UtcDateAndTime().Millisecond;
            if (milliseconds < 500)
            {
                return new GameStartedEvent(_id, 1, _senderId, _receiverId);
            }
            return new GameStartedEvent(_id, 1, _receiverId, _senderId);
        }

        private static Exception NewChallengeAlreadyAcceptedException()
        {
            return new DomainException(DomainExceptionMessages.Challenges_AlreadyAccepted);
        }

        private static Exception NewChallengeAlreadyRejectedException()
        {
            return new DomainException(DomainExceptionMessages.Challenges_AlreadyRejected);
        }        
    }
}

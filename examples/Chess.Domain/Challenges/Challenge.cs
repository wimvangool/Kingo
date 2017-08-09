using System;
using Kingo.Clocks;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Games;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Challenges
{    
    public sealed class Challenge : AggregateRoot<Guid, int>
    {        
        private readonly Guid _senderId;
        private readonly Guid _receiverId;
        private ChallengeState _state;        

        internal Challenge(PlayerChallengedEvent @event)
            : base(@event)
        {            
            _senderId = @event.SenderId;
            _receiverId = @event.ReceiverId;
            _state = ChallengeState.Pending;            
        }

        protected override int NextVersion() =>
            Version + 1;

        protected override ISnapshot<Guid, int> TakeSnapshot()
        {
            throw new NotImplementedException();
        }

        public void Accept()
        {
            if (!_receiverId.Equals(Session.Current.UserId))
            {
                throw NewPlayerCannotAcceptChallengeException(Session.Current.UserName);                
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

            Publish(new ChallengeAcceptedEvent());                
        }

        private static Exception NewPlayerCannotAcceptChallengeException(string playerName)
        {
            var messageFormat = ExceptionMessages.Challenges_PlayerCannotAcceptChallenge;
            var message = string.Format(messageFormat, playerName);
            return new IllegalOperationException(message);
        }

        public void Reject()
        {
            if (!_receiverId.Equals(Session.Current.UserId))
            {
                throw NewPlayerCannotAcceptChallengeException(Session.Current.UserName);
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

            Publish(new ChallengeRejectedEvent());                
        }

        public Game StartGame() =>
            new Game(CreateGameStartedEvent());

        private GameStartedEvent CreateGameStartedEvent()
        {
            // We leave the assignment of white and black up to chance.
            var milliseconds = Clock.Current.UtcDateAndTime().Millisecond;
            if (milliseconds < 500)
            {
                return new GameStartedEvent(Guid.NewGuid(), 1, _senderId, _receiverId);
            }
            return new GameStartedEvent(Guid.NewGuid(), 1, _receiverId, _senderId);
        }

        private static Exception NewChallengeAlreadyAcceptedException() =>
            new IllegalOperationException(ExceptionMessages.Challenges_AlreadyAccepted);

        private static Exception NewChallengeAlreadyRejectedException() =>
            new IllegalOperationException(ExceptionMessages.Challenges_AlreadyRejected);
    }
}

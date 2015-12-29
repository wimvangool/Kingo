using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Challenges
{
    [Serializable]
    public sealed class Challenge : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private readonly Guid _senderId;
        private readonly Guid _receiverId;
        private int _version;

        internal Challenge(PlayerChallengedEvent @event)
            : base(@event)
        {
            _id = @event.ChallengeId;
            _senderId = @event.SenderId;
            _receiverId = @event.ReceiverId;
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
            if (_receiverId.Equals(Session.Current.PlayerId))
            {
                Publish(new ChallengeAcceptedEvent(_id, NextVersion()));
                return;
            }
            throw NewPlayerCannotAcceptChallengeException(Session.Current.PlayerName);
        }

        private static Exception NewPlayerCannotAcceptChallengeException(string playerName)
        {
            var messageFormat = DomainExceptionMessages.Challenges_PlayerCannotAcceptChallenge;
            var message = string.Format(messageFormat, playerName);
            return new DomainException(message);
        }
    }
}

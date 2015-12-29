using System;
using Kingo.Messaging.Domain;

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
    }
}

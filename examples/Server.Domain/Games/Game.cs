using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    [Serializable]
    public sealed class Game : AggregateEventStream<Guid, int>
    {
        private Guid _id;
        private int _version;
        private Guid _whitePlayerId;
        private Guid _blackPlayerId;
        private bool _hasEnded;

        internal Game(GameStartedEvent @event)
            : base(@event)
        {
            Handle(@event);            
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

        public void Forfeit()
        {
            var playerId = Session.Current.PlayerId;

            if (IsNoPlayer(playerId))
            {
                throw NewIsNoPlayerException(playerId);
            }
            if (_hasEnded)
            {
                throw NewGameAlreadyEndedException();
            }
            Publish(new GameForfeitedEvent(Id, NextVersion()));
        }                

        private bool IsNoPlayer(Guid playerId)
        {
            return !playerId.Equals(_whitePlayerId) && !playerId.Equals(_blackPlayerId);
        }

        protected override void RegisterEventHandlers()
        {
            RegisterEventHandler<GameStartedEvent>(Handle);
            RegisterEventHandler<GameForfeitedEvent>(Handle);
        }

        private void Handle(GameStartedEvent @event)
        {
            _id = @event.GameId;
            _version = @event.GameVersion;
            _whitePlayerId = @event.WhitePlayerId;
            _blackPlayerId = @event.BlackPlayerId;
        }      
  
        private void Handle(GameForfeitedEvent @event)
        {
            _hasEnded = true;
        }

        private static Exception NewIsNoPlayerException(Guid playerId)
        {
            return DomainException.CreateException(DomainExceptionMessages.Game_SenderNoPlayer, playerId);
        }

        private static Exception NewGameAlreadyEndedException()
        {
            return new DomainException(DomainExceptionMessages.Game_GameAlreadyEnded);
        }
    }
}

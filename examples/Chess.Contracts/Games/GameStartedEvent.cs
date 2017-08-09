using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameStartedEvent : Event<Guid, int>
    {
        public GameStartedEvent(Guid whitePlayerId,  Guid blackPlayerId)
        {
            GameId = Guid.NewGuid();
            GameVersion = 1;

            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }

        [DataMember, AggregateId]
        public Guid GameId
        {
            get;
            private set;
        }

        [DataMember, AggregateVersion]
        public int GameVersion
        {
            get;
            private set;
        }

        [DataMember]
        public Guid WhitePlayerId
        {
            get;
            private set;
        }

        [DataMember]
        public Guid BlackPlayerId
        {
            get;
            private set;
        }
    }
}

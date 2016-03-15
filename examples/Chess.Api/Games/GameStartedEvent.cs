using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameStartedEvent : DomainEvent<Guid, int>
    {
        public GameStartedEvent(Guid gameId, int gameVersion, Guid whitePlayerId,  Guid blackPlayerId)
        {
            GameId = gameId;
            GameVersion = gameVersion;

            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }

        [DataMember, Key]
        public Guid GameId
        {
            get;
            private set;
        }

        [DataMember, Version]
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

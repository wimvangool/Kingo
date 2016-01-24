using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;
using NServiceBus;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameStartedEvent : DomainEvent, IEvent
    {
        [DataMember]        
        public readonly Guid GameId;

        [DataMember]        
        public readonly int GameVersion;

        [DataMember]
        public readonly Guid WhitePlayerId;

        [DataMember]
        public readonly Guid BlackPlayerId;

        public GameStartedEvent(Guid gameId, int gameVersion, Guid whitePlayerId,  Guid blackPlayerId)
        {
            GameId = gameId;
            GameVersion = gameVersion;

            WhitePlayerId = whitePlayerId;
            BlackPlayerId = blackPlayerId;
        }
    }
}

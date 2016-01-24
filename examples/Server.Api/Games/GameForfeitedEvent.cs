using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameForfeitedEvent : DomainEvent
    {
        [DataMember]
        public readonly Guid GameId;

        [DataMember]
        public readonly int GameVersion;

        public GameForfeitedEvent(Guid gameId, int gameVersion)
        {
            GameId = gameId;
            GameVersion = gameVersion;
        }
    }
}

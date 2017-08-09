using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameForfeitedEvent : Event<Guid, int>
    {                
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
    }
}

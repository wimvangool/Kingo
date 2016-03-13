using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GameForfeitedEvent : DomainEvent<Guid, int>
    {        
        //public GameForfeitedEvent(Guid gameId, int gameVersion)
        //{
        //    //GameId = gameId;
        //    //GameVersion = gameVersion;
        //}

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
    }
}

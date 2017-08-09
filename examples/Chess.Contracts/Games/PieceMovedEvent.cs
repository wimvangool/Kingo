using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public class PieceMovedEvent : Event<Guid, int>
    {
        public PieceMovedEvent(string from, string to)
        {
            From = from;
            To = to;            
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
        public string From
        {
            get;
            private set;
        }

        [DataMember]
        public string To
        {
            get;
            private set;
        }                      

        [DataMember]
        public GameState NewState
        {
            get;
            set;
        }
    }
}

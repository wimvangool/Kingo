using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class PieceMovedEvent : DomainEvent<Guid, int>
    {
        public PieceMovedEvent(string from, string to)
        {
            From = from;
            To = to;            
        }

        [DataMember, Key]
        public Guid GameId
        {
            get;
            set;
        }

        [DataMember, Version]
        public int GameVersion
        {
            get;
            set;
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
        public string EnPassantHit
        {
            get;
            set;
        }  

        [DataMember]
        public GameState NewState
        {
            get;
            set;
        }
    }
}

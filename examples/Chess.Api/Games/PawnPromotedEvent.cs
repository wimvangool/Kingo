using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class PawnPromotedEvent : DomainEvent<Guid, int>
    {
        public PawnPromotedEvent(string pawnPosition, TypeOfPiece promotedTo)
        {
            PawnPosition = pawnPosition;
            PromotedTo = promotedTo;            
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
        public string PawnPosition
        {
            get;
            private set;
        }

        [DataMember]
        public TypeOfPiece PromotedTo
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

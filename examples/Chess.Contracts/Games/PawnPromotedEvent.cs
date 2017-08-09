using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class PawnPromotedEvent : Event<Guid, int>
    {
        public PawnPromotedEvent(string pawnPosition, TypeOfPiece promotedTo)
        {
            PawnPosition = pawnPosition;
            PromotedTo = promotedTo;            
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

using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class EnPassantHitEvent : PieceMovedEvent
    {
        public EnPassantHitEvent(string from, string to)
            : base(from, to) { }

        [DataMember]
        public string EnPassantHit
        {
            get;
            set;
        }
    }
}

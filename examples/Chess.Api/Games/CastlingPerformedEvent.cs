using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class CastlingPerformedEvent : PieceMovedEvent
    {
        public CastlingPerformedEvent(string from, string to)
            : base(from, to) { }

        [DataMember]
        public string RookFrom
        {
            get;
            set;
        }

        [DataMember]
        public string RookTo
        {
            get;
            set;
        }
    }
}

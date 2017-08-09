using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class PawnMovedToEightRankEvent : PieceMovedEvent
    {
        public PawnMovedToEightRankEvent(string from, string to)
            : base(from, to) { }
    }
}

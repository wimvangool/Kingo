using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class King : Piece
    {
        public King(Game game, ColorOfPiece color)
        {
            EventBus = game;  
            Color = color;
        }

        protected override IDomainEventBus<Guid, int> EventBus
        {
            get;
        }

        protected override ColorOfPiece Color
        {
            get;
        }

        protected override TypeOfPiece Type
        {
            get { return TypeOfPiece.King; }
        }

        public override bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, from, to, ref eventFactory))
            {
                return IsSupportedMove(Square.CalculateMove(from, to));
            }
            return false;
        }

        private static bool IsSupportedMove(Move move)
        {
            return IsAtMostOneStep(move.FileSteps) && IsAtMostOneStep(move.RankSteps);
        }

        private static bool IsAtMostOneStep(int steps)
        {
            return Math.Abs(steps) <= 1;
        }
    }
}

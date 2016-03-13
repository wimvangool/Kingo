using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Rook : Piece
    {
        public Rook(Game game, ColorOfPiece color)
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

        protected override bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Square enPassantHit)
        {
            if (base.IsSupportedMove(board, from, to, ref enPassantHit))
            {                
                return IsSupportedMove(board, Square.CalculateMove(from, to));
            }
            return false;
        }

        private static bool IsSupportedMove(ChessBoard board, Move move)
        {
            return move.IsStraightPath && move.IsEmptyPath(board);
        }
    }
}

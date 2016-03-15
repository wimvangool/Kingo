using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Knight : Piece
    {
        public Knight(Game game, ColorOfPiece color)
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

        protected override bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, from, to, ref eventFactory))
            {
                return IsSupportedMove(Square.CalculateMove(from, to));
            }
            return false;
        }

        private static bool IsSupportedMove(Move move)
        {
            return
                IsTwoUpOneRight(move) ||
                IsOneUpTwoRight(move) ||
                IsOneDownTwoRight(move) ||
                IsTwoDownOneRight(move) ||
                IsTwoDownOneLeft(move) ||
                IsOneDownTwoLeft(move) ||
                IsOneUpTwoLeft(move) ||
                IsTwoUpOneLeft(move);
        }

        private const int _Two = 2;
        private const int _One = 1;

        private static bool IsTwoUpOneRight(Move move)
        {
            return IsMove(move, _Two, _One);
        }

        private static bool IsOneUpTwoRight(Move move)
        {
            return IsMove(move, _One, _Two);
        }

        private static bool IsOneDownTwoRight(Move move)
        {
            return IsMove(move, -_One, _Two);
        }

        private static bool IsTwoDownOneRight(Move move)
        {
            return IsMove(move, -_Two, _One);
        }

        private static bool IsTwoDownOneLeft(Move move)
        {
            return IsMove(move, -_Two, -_One);
        }

        private static bool IsOneDownTwoLeft(Move move)
        {
            return IsMove(move, -_One, -_Two);
        }

        private static bool IsOneUpTwoLeft(Move move)
        {
            return IsMove(move, _One, -_Two);    
        }

        private static bool IsTwoUpOneLeft(Move move)
        {
            return IsMove(move, _Two, -_One);
        }

        private static bool IsMove(Move move, int rankSteps, int fileSteps)
        {
            return move.RankSteps == rankSteps && move.FileSteps == fileSteps;
        }
    }
}

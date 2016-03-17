using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override TypeOfPiece Type
        {
            get { return TypeOfPiece.Rook; }
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            return PossibleSquaresToMoveTo(from);
        }

        internal static IEnumerable<Square> PossibleSquaresToMoveTo(Square from)
        {
            return PossibleMovesUp(from)
                .Concat(PossibleMovesRight(from))
                .Concat(PossibleMovesDown(from))
                .Concat(PossibleMovesLeft(from));
        }

        private static IEnumerable<Square> PossibleMovesUp(Square from)
        {
            int rankSteps = 1;
            Square to;

            while (from.TryAdd(0, rankSteps, out to))
            {
                rankSteps++;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesRight(Square from)
        {
            int fileSteps = 1;
            Square to;

            while (from.TryAdd(fileSteps, 0, out to))
            {
                fileSteps++;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesDown(Square from)
        {
            int rankSteps = -1;
            Square to;

            while (from.TryAdd(0, rankSteps, out to))
            {
                rankSteps--;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesLeft(Square from)
        {
            int fileSteps = -1;
            Square to;

            while (from.TryAdd(fileSteps, 0, out to))
            {
                fileSteps--;

                yield return to;
            }
        }

        protected override bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, from, to, ref eventFactory))
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

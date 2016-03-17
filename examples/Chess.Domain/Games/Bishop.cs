using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Bishop : Piece
    {
        public Bishop(Game game, ColorOfPiece color)
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
            get { return TypeOfPiece.Bishop; }
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            return PossibleSquaresToMoveTo(from);
        }

        internal static IEnumerable<Square> PossibleSquaresToMoveTo(Square from)
        {
            return PossibleMovesUpLeft(from)
                .Concat(PossibleMovesUpRight(from))
                .Concat(PossibleMovesDownRight(from))
                .Concat(PossibleMovesDownLeft(from));
        }

        private static IEnumerable<Square> PossibleMovesUpLeft(Square from)
        {
            int fileSteps = -1;
            int rankSteps = 1;
            Square to;

            while (from.TryAdd(fileSteps, rankSteps, out to))
            {
                fileSteps--;
                rankSteps++;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesUpRight(Square from)
        {
            int fileSteps = 1;
            int rankSteps = 1;
            Square to;

            while (from.TryAdd(fileSteps, rankSteps, out to))
            {
                fileSteps++;
                rankSteps++;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesDownRight(Square from)
        {
            int fileSteps = 1;
            int rankSteps = -1;
            Square to;

            while (from.TryAdd(fileSteps, rankSteps, out to))
            {
                fileSteps++;
                rankSteps--;

                yield return to;
            }
        }

        private static IEnumerable<Square> PossibleMovesDownLeft(Square from)
        {
            int fileSteps = -1;
            int rankSteps = -1;
            Square to;

            while (from.TryAdd(fileSteps, rankSteps, out to))
            {
                fileSteps--;
                rankSteps--;

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
            return move.IsCrossPath && move.IsEmptyPath(board);
        }
    }
}

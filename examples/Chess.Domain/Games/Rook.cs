using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Rook : ChessPiece
    {
        private readonly bool _hasMoved;

        public Rook(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color)
        {
            EventBus = eventBus;
            Color = color;
        }

        internal Rook(IDomainEventBus<Guid, int>  eventBus, ColorOfPiece color, bool hasMoved)
        {
            EventBus = eventBus;
            Color = color;

            _hasMoved = hasMoved;
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

        public override bool CanTakePartInCastlingMove
        {
            get { return !_hasMoved; }
        }

        public override ChessPiece ApplyMove(Move move)
        {
            return new Rook(EventBus, Color, true);
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

        protected override bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, move, ref eventFactory))
            {
                return move.IsStraightPath && move.IsEmptyPath(board);
            }
            return false;
        }        
    }
}

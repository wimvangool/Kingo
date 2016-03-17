using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            var steps = Enumerable.Range(-1, 3);

            foreach (var fileSteps in steps)
            {
                foreach (var rankSteps in steps)
                {
                    Square to;

                    if (from.TryAdd(fileSteps, rankSteps, out to) && !from.Equals(to))
                    {
                        yield return to;
                    }
                }
            }
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
            return IsAtMostOneStep(move.FileSteps) && IsAtMostOneStep(move.RankSteps);
        }

        private static bool IsAtMostOneStep(int steps)
        {
            return Math.Abs(steps) <= 1;
        }
    }
}

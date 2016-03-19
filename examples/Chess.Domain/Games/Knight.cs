using System;
using System.Collections.Generic;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Knight : Piece
    {
        private static readonly Tuple<int, int> _A_TwoUpOneRight = new Tuple<int, int>(1, 2);
        private static readonly Tuple<int, int> _B_OneUpTwoRight = new Tuple<int, int>(2, 1);
        private static readonly Tuple<int, int> _C_OneDownTwoRight = new Tuple<int, int>(2, -1);
        private static readonly Tuple<int, int> _D_TwoDownOneRight = new Tuple<int, int>(1, -2);
        private static readonly Tuple<int, int> _E_TwoDownOneLeft = new Tuple<int, int>(-1, -2);
        private static readonly Tuple<int, int> _F_OneDownTwoLeft = new Tuple<int, int>(-2, -1);
        private static readonly Tuple<int, int> _G_OneUpTwoLeft = new Tuple<int, int>(-2, 1);
        private static readonly Tuple<int, int> _H_TwoUpOneLeft = new Tuple<int, int>(-1, 2);

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

        protected override TypeOfPiece Type
        {
            get { return TypeOfPiece.Knight; }
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            Square to;

            if (TryAdd(from, _A_TwoUpOneRight, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _B_OneUpTwoRight, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _C_OneDownTwoRight, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _D_TwoDownOneRight, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _E_TwoDownOneLeft, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _F_OneDownTwoLeft, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _G_OneUpTwoLeft, out to))
            {
                yield return to;
            }
            if (TryAdd(from, _H_TwoUpOneLeft, out to))
            {
                yield return to;
            }
        }        

        protected override bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, move, ref eventFactory))
            {
                return
                    IsMove(move, _A_TwoUpOneRight) ||
                    IsMove(move, _B_OneUpTwoRight) ||
                    IsMove(move, _C_OneDownTwoRight) ||
                    IsMove(move, _D_TwoDownOneRight) ||
                    IsMove(move, _E_TwoDownOneLeft) ||
                    IsMove(move, _F_OneDownTwoLeft) ||
                    IsMove(move, _G_OneUpTwoLeft) ||
                    IsMove(move, _H_TwoUpOneLeft);
            }
            return false;
        }        

        private static bool TryAdd(Square from, Tuple<int, int> move, out Square to)
        {
            return from.TryAdd(move.Item1, move.Item2, out to);
        }

        private static bool IsMove(Move move, Tuple<int, int> steps)
        {
            return IsMove(move, steps.Item1, steps.Item2);
        }

        private static bool IsMove(Move move, int fileSteps, int rankSteps)
        {
            return move.FileSteps == fileSteps && move.RankSteps == rankSteps;
        }
    }
}

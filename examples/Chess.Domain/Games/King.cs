using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class King : ChessPiece
    {        
        private readonly bool _hasMoved;

        public King(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color)
        {
            EventBus = eventBus;  
            Color = color;
        }

        private King(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color, bool hasMoved)
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
            get { return TypeOfPiece.King; }
        }

        public override bool CanTakePartInCastlingMove
        {
            get { return !_hasMoved; }
        }

        public override ChessPiece ApplyMove(Move move)
        {
            return new King(EventBus, Color, true);
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            // First, the regular steps of a King are returned.
            var steps = Enumerable.Range(-1, 3);
            Square to;

            foreach (var fileSteps in steps)
            {
                foreach (var rankSteps in steps)
                {                    
                    if (from.TryAdd(fileSteps, rankSteps, out to) && !from.Equals(to))
                    {
                        yield return to;
                    }
                }
            }

            // Second, the possible Castling-moves are returned.
            if (from.TryAdd(-2, 0, out to))
            {
                yield return to;
            }
            if (from.TryAdd(2, 0, out to))
            {
                yield return to;
            }
        }

        protected override bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, move, ref eventFactory))
            {
                return IsOneStepMove(move) || IsValidCastlingMove(board, move, ref eventFactory);
            }
            return false;
        }        

        #region [====== One Step ======]

        private static bool IsOneStepMove(Move move)
        {
            return IsAtMostOneStep(move.FileSteps) && IsAtMostOneStep(move.RankSteps);
        }

        private static bool IsAtMostOneStep(int steps)
        {
            return Math.Abs(steps) <= 1;
        }

        #endregion

        #region [====== Castling Moves ======] 

        private static readonly Move[] _CastlingShortOfWhiteMoves =
        {
            Games.Move.Calculate(Square.Parse("e1"), Square.Parse("g1")),
            Games.Move.Calculate(Square.Parse("h1"), Square.Parse("f1"))
        };

        private static readonly Move[] _CastlingLongOfWhiteMoves =
        {
            Games.Move.Calculate(Square.Parse("e1"), Square.Parse("c1")),
            Games.Move.Calculate(Square.Parse("a1"), Square.Parse("d1"))
        };

        private static readonly Move[] _CastlingShortOfBlackMoves =
        {
            Games.Move.Calculate(Square.Parse("e8"), Square.Parse("g8")),
            Games.Move.Calculate(Square.Parse("h8"), Square.Parse("f8"))
        };

        private static readonly Move[] _CastlingLongOfBlackMoves =
        {
            Games.Move.Calculate(Square.Parse("e8"), Square.Parse("c8")),
            Games.Move.Calculate(Square.Parse("a8"), Square.Parse("d8"))
        };

        private bool IsValidCastlingMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (CanTakePartInCastlingMove)
            {
                if (Color == ColorOfPiece.White)
                {
                    return IsValidCastlingMoveOfWhite(board, move, ref eventFactory);
                }
                if (Color == ColorOfPiece.Black)
                {
                    return IsValidCastlingMoveOfBlack(board, move, ref eventFactory);
                }
            }            
            return false;
        }

        private bool IsValidCastlingMoveOfWhite(ChessBoard board, Move moveOfKing, ref Func<PieceMovedEvent> eventFactory)
        {
            if (moveOfKing.Equals(_CastlingShortOfWhiteMoves[0]))
            {
                return IsValidCastlingMove(board, moveOfKing, _CastlingShortOfWhiteMoves[1], ref eventFactory);
            }
            if (moveOfKing.Equals(_CastlingLongOfWhiteMoves[0]))
            {
                return IsValidCastlingMove(board, moveOfKing, _CastlingLongOfWhiteMoves[1], ref eventFactory);
            }
            return false;
        }

        private bool IsValidCastlingMoveOfBlack(ChessBoard board, Move moveOfKing, ref Func<PieceMovedEvent> eventFactory)
        {
            if (moveOfKing.Equals(_CastlingShortOfBlackMoves[0]))
            {
                return IsValidCastlingMove(board, moveOfKing, _CastlingShortOfBlackMoves[1], ref eventFactory);
            }
            if (moveOfKing.Equals(_CastlingLongOfBlackMoves[0]))
            {
                return IsValidCastlingMove(board, moveOfKing, _CastlingLongOfBlackMoves[1], ref eventFactory);
            }
            return false;
        }

        private bool IsValidCastlingMove(ChessBoard board, Move moveOfKing, Move moveOfRook, ref Func<PieceMovedEvent> eventFactory)
        {            
            if (moveOfKing.IsEmptyPath(board) && !IsUnderAttack(board, moveOfKing, Color.Invert()))
            {
                var piece = board.SelectPiece(moveOfRook.From);
                if (piece != null && piece.CanTakePartInCastlingMove && moveOfRook.IsEmptyPath(board))
                {
                    eventFactory = () => new CastlingPerformedEvent(moveOfKing.From.ToString(), moveOfKing.To.ToString())
                    {
                        RookFrom = moveOfRook.From.ToString(),
                        RookTo = moveOfRook.To.ToString(),
                        NewState = board.SimulateCastlingMove(moveOfKing, moveOfRook, Color)
                    };
                    return true;
                }
            }
            return false;
        }

        private static bool IsUnderAttack(ChessBoard board, Move moveOfKing, ColorOfPiece colorOfOpponent)
        {
            IEnumerable<Square> path;

            if (moveOfKing.TryGetSquaresInStraightLine(out path))
            {
                return
                    board.CanAnyPieceMoveTo(moveOfKing.From, colorOfOpponent) ||
                    board.CanAnyPieceMoveTo(path.Single(), colorOfOpponent);
            }
            return false;
        }

        #endregion
    }
}

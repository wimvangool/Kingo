using System;
using System.Collections.Generic;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Pawn : ChessPiece
    {        
        public Pawn(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color)
        {
            EventBus = eventBus;
            Color = color;
        }

        private Pawn(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color, bool canBeHitByEnPassantMove)
        {
            EventBus = eventBus;
            Color = color;
            CanBeHitByEnPassantMove = canBeHitByEnPassantMove;
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
            get { return TypeOfPiece.Pawn; }
        }

        #region [====== Move ======]

        public override bool CanBeHitByEnPassantMove
        {
            get;
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            Square to;

            if (from.TryAdd(-1, 1, out to))
            {
                yield return to;
            }
            if (from.TryAdd(0, 1, out to))
            {
                yield return to;
            }
            if (from.TryAdd(0, 2, out to))
            {
                yield return to;
            }
            if (from.TryAdd(1, 1, out to))
            {
                yield return to;
            }
        }

        protected override bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, move, ref eventFactory) && IsSupportedMoveOfPawn(board, move, ref eventFactory))
            {                
                if (move.To.IsEightRank(Color))
                {
                    eventFactory = () => new PawnMovedToEightRankEvent(move.From.ToString(), move.To.ToString())
                    {
                        NewState = GameState.AwaitingPawnPromotion
                    };
                }
                return true;
            }
            return false;
        }

        private bool IsSupportedMoveOfPawn(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (IsOneStepForward(move))
            {
                return board.IsEmpty(move.To);
            }
            if (IsTwoStepForward(move) && IsFirstPawnMove(move.From))
            {
                return board.IsEmpty(move.To) && move.IsEmptyPath(board);
            }
            if (IsHitMove(move))
            {
                if (ContainsOpponentPiece(board, move.To))
                {
                    return true;
                }
                Square removedPawn;

                if (IsEnPassantHit(board, move.To, out removedPawn))
                {
                    eventFactory = () => new EnPassantHitEvent(move.From.ToString(), move.To.ToString())
                    {
                        EnPassantHit = removedPawn.ToString(),
                        NewState = board.SimulateEnPassantMove(move, removedPawn, Color)
                    };
                    return true;
                }
            }
            return false;
        }

        public override ChessPiece RemainInPlace()
        {
            return new Pawn(EventBus, Color, false);
        }

        public override ChessPiece ApplyMove(Move move)
        {
            if (IsTwoStepForward(move))
            {
                return new Pawn(EventBus, Color, true);
            }
            return this;
        }

        private bool IsOneStepForward(Move move)
        {
            return IsStepForward(move, 1);
        }

        private bool IsTwoStepForward(Move move)
        {
            return IsStepForward(move, 2);
        }

        private bool IsStepForward(Move move, int stepCount)
        {
            if (move.FileSteps == 0)
            {
                return IsMovingForwardOnRank(move, stepCount);
            }
            return false;
        }

        private bool IsFirstPawnMove(Square from)
        {
            return Color == ColorOfPiece.White
                ? from.Rank == 2
                : from.Rank == 7;
        }

        private bool IsHitMove(Move move)
        {
            if (move.FileSteps == -1 || move.FileSteps == 1)
            {
                return IsMovingForwardOnRank(move, 1);
            }
            return false;
        }

        private bool IsMovingForwardOnRank(Move move, int stepCount)
        {
            return Color == ColorOfPiece.White
                ? move.RankSteps == stepCount
                : move.RankSteps == -stepCount;
        }

        private bool ContainsOpponentPiece(ChessBoard board, Square to)
        {
            var piece = board.SelectPiece(to);
            if (piece == null)
            {
                return false;
            }
            return piece.HasColor(Color.Invert());
        }

        private bool IsEnPassantHit(ChessBoard board, Square to, out Square removedPawn)
        {
            var oneStepBack = Color == ColorOfPiece.White ? -1 : 1;
            var opponentSquare = to.Add(0, oneStepBack);

            var piece = board.SelectPiece(opponentSquare);
            if (piece == null || piece.HasColor(Color) || !piece.CanBeHitByEnPassantMove)
            {
                removedPawn = null;
                return false;
            }
            removedPawn = opponentSquare;
            return true;
        }

        #endregion

        #region [====== Pawn Promotion ======]

        public override void PromoteTo(ChessBoard board, Square position, TypeOfPiece promoteTo)
        {
            if (promoteTo == TypeOfPiece.Pawn || promoteTo == TypeOfPiece.King)
            {
                throw NewInvalidPromotionException(promoteTo);
            }
            EventBus.Publish(new PawnPromotedEvent(position.ToString(), promoteTo)
            {
                NewState = board.SimulatePawnPromotion(position, promoteTo, Color)
            });
        }

        public override ChessPiece ApplyPromotion(TypeOfPiece promoteTo)
        {
            switch (promoteTo)
            {
                case TypeOfPiece.Bishop:
                    return new Bishop(EventBus, Color);

                case TypeOfPiece.Knight:
                    return new Knight(EventBus, Color);

                case TypeOfPiece.Queen:
                    return new Queen(EventBus, Color);

                case TypeOfPiece.Rook:
                    return new Rook(EventBus, Color, true);

                default:
                    return base.ApplyPromotion(promoteTo);
            }            
        }

        private static Exception NewInvalidPromotionException(TypeOfPiece promoteTo)
        {
            return new ArgumentOutOfRangeException(nameof(promoteTo), $"Pawn cannot be promoted to a '{promoteTo}'.");
        }

        #endregion
    }
}

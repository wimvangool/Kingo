using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Pawn : Piece
    {        
        public Pawn(Game game, ColorOfPiece color)
        {
            EventBus = game;
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

        public override bool CanBeHitByEnPassantMove
        {
            get;
        }

        protected override bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, from, to, ref eventFactory))
            {
                var move = Square.CalculateMove(from, to);

                if (IsOneStepForward(move))
                {
                    return board.IsEmpty(move.To);
                }
                if (IsTwoStepForward(move) && IsFirstPawnMove(from))
                {
                    return board.IsEmpty(move.To) && move.IsEmptyPath(board);
                }
                if (IsHitMove(move))
                {
                    if (ContainsOpponentPiece(board, to))
                    {
                        return true;
                    }
                    Square removedPawn;

                    if (IsEnPassantHit(board, to, out removedPawn))
                    {
                        eventFactory = () => new EnPassantHitEvent(from.ToString(), to.ToString())
                        {
                            EnPassantHit = removedPawn.ToString(),
                            NewState = board.SimulateEnPassantMove(from, to, removedPawn, Color)
                        };
                        return true;
                    }
                }
            }
            return false;
        }

        public override Piece RemainInPlace()
        {
            return new Pawn(EventBus, Color, false);
        }

        public override Piece ApplyMove(Square from, Square to)
        {
            if (IsTwoStepForward(Square.CalculateMove(from, to)))
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
    }
}

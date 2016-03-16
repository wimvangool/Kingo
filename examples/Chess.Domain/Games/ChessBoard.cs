using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class ChessBoard
    {
        internal const int Size = 8;
        
        private readonly Piece[,] _pieces;

        private ChessBoard(Piece[,] pieces)
        {            
            _pieces = pieces;
        }

        #region [====== ToString ======]

        public override string ToString()
        {
            return $"W: {CountPieces(ColorOfPiece.White)} piece(s), B: {CountPieces(ColorOfPiece.Black)} piece(s)";
        }        

        private int CountPieces(ColorOfPiece color)
        {
            return EnumeratePieces(_pieces, color).Count();
        }

        private static IEnumerable<Tuple<Square, Piece>> EnumeratePieces(Piece[,] pieces, ColorOfPiece color)
        {
            return EnumeratePieces(pieces).Where(piece => piece.Item2.HasColor(color));
        }

        private static IEnumerable<Tuple<Square, Piece>> EnumeratePieces(Piece[,] pieces)
        {
            for (int fileIndex = 0; fileIndex < Size; fileIndex++)
            {
                for (int rankIndex = 0; rankIndex < Size; rankIndex++)
                {
                    var piece = pieces[fileIndex, rankIndex];
                    if (piece != null)
                    {
                        yield return new Tuple<Square, Piece>(new Square(fileIndex, rankIndex), piece);
                    }
                }
            }
        }

        #endregion

        #region [====== IsEmpty & SelectPiece ======]

        public bool IsEmpty(Square square)
        {
            return SelectPiece(square) == null;
        }

        public Piece SelectPiece(Square square)
        {
            return square.SelectPiece(_pieces);          
        }

        #endregion

        #region [====== MovePiece ======]

        public void MovePiece(Square from, Square to, ColorOfPiece color)
        {
            var piece = SelectPiece(from);
            if (piece == null)
            {
                throw NewEmptySquareException(from);
            }
            if (piece.HasColor(color))
            {
                piece.Move(this, from, to);
                return;
            }
            throw NewWrongColorException(from, color, color.Invert());
        }

        public GameState SimulateMove(Square from, Square to, ColorOfPiece colorOfOwnKing)
        {
            return DetermineNewState(Square.ApplyMove(from, to, _pieces), colorOfOwnKing);
        }

        public GameState SimulateEnPassantMove(Square from, Square to, Square enPassantHit, ColorOfPiece colorOfOwnKing)
        {
            return DetermineNewState(Square.ApplyEnPassantMove(from, to, enPassantHit, _pieces), colorOfOwnKing);
        }

        private static GameState DetermineNewState(Piece[,] piecesAfterMove, ColorOfPiece colorOfOwnKing)
        {
            if (IsInCheck(colorOfOwnKing, piecesAfterMove))
            {
                return GameState.Error;
            }
            var colorOfOtherKing = colorOfOwnKing.Invert();

            if (IsInCheck(colorOfOtherKing, piecesAfterMove))
            {
                if (IsCheckMate(colorOfOtherKing, piecesAfterMove))
                {
                    return GameState.CheckMate;
                }
                return GameState.Check;
            }
            if (IsStaleMate(colorOfOwnKing, piecesAfterMove))
            {
                return GameState.StaleMate;
            }
            return GameState.Normal;
        }           

        private static bool IsInCheck(ColorOfPiece colorOfKing, Piece[,] pieces)
        {
            return CanAnyPieceMoveTo(FindSquareOfKing(colorOfKing, pieces), pieces, colorOfKing.Invert());
        }

        private static Square FindSquareOfKing(ColorOfPiece colorOfKing, Piece[,] pieces)
        {
            var piecesOnBoard =
                from pieceOnBoard in EnumeratePieces(pieces, colorOfKing)
                where pieceOnBoard.Item2.IsOfType(TypeOfPiece.King)
                select pieceOnBoard.Item1;

            return piecesOnBoard.Single();            
        }

        private static bool CanAnyPieceMoveTo(Square to, Piece[,] pieces, ColorOfPiece colorOfPiece)
        {
            Func<PieceMovedEvent> eventFactory = null;
            var board = new ChessBoard(pieces);

            foreach (var pieceOnBoard in EnumeratePieces(pieces, colorOfPiece))
            {                
                var from = pieceOnBoard.Item1;
                var piece = pieceOnBoard.Item2;
                if (piece.IsSupportedMove(board, from, to, ref eventFactory))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsCheckMate(ColorOfPiece colorOfKing, Piece[,] pieces)
        {
            return false;
        }

        private static bool IsStaleMate(ColorOfPiece colorOfKing, Piece[,] pieces)
        {
            return false;
        }

        private static Exception NewEmptySquareException(Square square)
        {
            var messageFormat = ExceptionMessages.Game_EmptySquare;
            var message = string.Format(messageFormat, square);
            return new DomainException(message);
        }

        private static Exception NewWrongColorException(Square square, ColorOfPiece expectedColor, ColorOfPiece actualColor)
        {
            var messageFormat = ExceptionMessages.Game_WrongColor;
            var message = string.Format(messageFormat, square, expectedColor, actualColor);
            return new DomainException(message);
        }        

        #endregion

        #region [====== ApplyMove ======]

        public ChessBoard ApplyMove(Square from, Square to)
        {
            return new ChessBoard(Square.ApplyMove(from, to, _pieces));
        }

        public ChessBoard ApplyEnPassantMove(Square from, Square to, Square enPassantHit)
        {
            return new ChessBoard(Square.ApplyEnPassantMove(from, to, enPassantHit, _pieces));
        }

        #endregion

        #region [====== New Game ======]

        public static ChessBoard SetupNewGame(Game game)
        {
            var pieces = new Piece[Size, Size];

            AddWhitePiecesTo(game, pieces);
            AddBlackPiecesTo(game, pieces);

            return new ChessBoard(pieces);
        } 

        private static void AddWhitePiecesTo(Game game, Piece[,] pieces)
        {            
            AddRegularPiecesTo(game, pieces, ColorOfPiece.White, 0);
            AddPawnsTo(game, pieces, ColorOfPiece.White, 1);           
        }

        private static void AddBlackPiecesTo(Game game, Piece[,] pieces)
        {
            AddRegularPiecesTo(game, pieces, ColorOfPiece.Black, 7);
            AddPawnsTo(game, pieces, ColorOfPiece.Black, 6);
        }

        private static void AddPawnsTo(Game game, Piece[,] pieces, ColorOfPiece color, int rankIndex)
        {
            for (int fileIndex = 0; fileIndex < Size; fileIndex++)
            {
                pieces[fileIndex, rankIndex] = new Pawn(game, color);
            }
        }
        
        private static void AddRegularPiecesTo(Game game, Piece[,] pieces, ColorOfPiece color, int rankIndex)
        {
            pieces[0, rankIndex] = new Rook(game, color);
            pieces[1, rankIndex] = new Knight(game, color);
            pieces[2, rankIndex] = new Bishop(game, color);
            pieces[3, rankIndex] = new Queen(game, color);
            pieces[4, rankIndex] = new King(game, color);
            pieces[5, rankIndex] = new Bishop(game, color);
            pieces[6, rankIndex] = new Knight(game, color);
            pieces[7, rankIndex] = new Rook(game, color);
        }

        #endregion
    }
}

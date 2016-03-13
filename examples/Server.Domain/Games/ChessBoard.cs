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
            return $"W: {CountWhitePieces()} piece(s), B: {CountBlackPieces()} piece(s)";
        }

        private int CountWhitePieces()
        {
            return CountPieces(ColorOfPiece.White);
        }

        private int CountBlackPieces()
        {
            return CountPieces(ColorOfPiece.Black);
        }

        private int CountPieces(ColorOfPiece color)
        {
            return EnumeratePieces().Count(piece => piece.HasColor(color));
        }

        private IEnumerable<Piece> EnumeratePieces()
        {
            for (int fileIndex = 0; fileIndex < Size; fileIndex++)
            {
                for (int rankIndex = 0; rankIndex < Size; rankIndex++)
                {
                    var piece = _pieces[fileIndex, rankIndex];
                    if (piece != null)
                    {
                        yield return piece;
                    }
                }
            }
        }

        #endregion

        #region [====== IsEmpty ======]

        public bool IsEmpty(Square square)
        {
            return SelectPiece(square) == null;
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

        public GameState DetermineNewState(Square from, Square to, Square enPassantHit, ColorOfPiece colorOfOwnKing)
        {
            var piecesAfterMove = Square.ApplyMove(from, to, enPassantHit, _pieces);

            if (IsInCheck(colorOfOwnKing, piecesAfterMove))
            {
                throw NewOwnKingLeftInCheckException(from, to);
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

        public ChessBoard ApplyMove(Square from, Square to, Square enPassantHit)
        {
            return new ChessBoard(Square.ApplyMove(from, to, enPassantHit, _pieces));
        }        

        private static bool IsInCheck(ColorOfPiece colorOfKing, Piece[,] pieces)
        {
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

        public Piece SelectPiece(Square square)
        {
            return square.SelectPiece(_pieces);          
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

        private static Exception NewOwnKingLeftInCheckException(Square from, Square to)
        {
            var messageFormat = ExceptionMessages.Game_OwnKingLeftInCheck;
            var message = string.Format(messageFormat, from, to);
            return new DomainException(message);
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

using System;
using System.Collections.Generic;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{
    [Serializable]
    public sealed class Square : ValueObject<Square>
    {
        private static readonly char[] _Files = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        private readonly int _fileIndex;
        private readonly int _rankIndex;

        private Square([Range(0, ChessBoard.Size - 1)] int fileIndex, [Range(0, ChessBoard.Size - 1)] int rankIndex)
        {
            _fileIndex = fileIndex;
            _rankIndex = rankIndex;
        }

        public char File
        {
            get { return _Files[_fileIndex]; }
        }

        public int Rank
        {
            get { return _rankIndex + 1; }
        }

        public override string ToString()
        {
            return $"{File}{Rank}";
        }

        internal IEnumerable<Square> CreateVerticalPath(int rankSteps)
        {
            foreach (var step in StepsInPath(rankSteps))
            {
                yield return new Square(_fileIndex, _rankIndex - step);
            }
        }

        internal IEnumerable<Square> CreateHorizontalPath(int fileSteps)
        {
            foreach (var step in StepsInPath(fileSteps))
            {
                yield return new Square(_fileIndex - step, _rankIndex);
            }
        }

        internal IEnumerable<Square> CreateCrossPath(int fileSteps, int rankSteps)
        {
            foreach (var step in StepsInPath(fileSteps, rankSteps))
            {
                yield return new Square(_fileIndex - step.Item1, _rankIndex - step.Item2);
            }
        }

        private static IEnumerable<Tuple<int, int>> StepsInPath(int fileSteps, int rankSteps)
        {
            using (var horizontalSteps = StepsInPath(fileSteps).GetEnumerator())
            using (var verticalSteps = StepsInPath(rankSteps).GetEnumerator())
            {
                while (horizontalSteps.MoveNext() && verticalSteps.MoveNext())
                {
                    yield return new Tuple<int, int>(horizontalSteps.Current, verticalSteps.Current);
                }
            }
        }

        private static IEnumerable<int> StepsInPath(int steps)
        {
            if (steps < 0)
            {
                for (int step = steps + 1; step < 0; step++)
                {
                    yield return step;
                }
            }
            if (steps > 0)
            {
                for (int step = steps - 1; step > 0; step--)
                {
                    yield return step;
                }
            }            
        }

        public Square Add(int fileSteps, int rankSteps)
        {
            return new Square(_fileIndex + fileSteps, _rankIndex + rankSteps);
        }

        internal Piece SelectPiece(Piece[,] pieces)
        {
            return pieces[_fileIndex, _rankIndex];            
        }

        internal static Move CalculateMove(Square from, Square to)
        {
            return new Move(to, to._fileIndex - from._fileIndex, to._rankIndex - from._rankIndex);
        }

        internal static Piece[,] ApplyMove(Square from, Square to, Square enPassantHit, Piece[,] pieces)
        {
            var piecesAfterMove = new Piece[ChessBoard.Size, ChessBoard.Size];

            for (int fileIndex = 0; fileIndex < ChessBoard.Size; fileIndex++)
            {
                for (int rankIndex = 0; rankIndex < ChessBoard.Size; rankIndex++)
                {
                    piecesAfterMove[fileIndex, rankIndex] = ApplyMove(from, to, enPassantHit, pieces, new Square(fileIndex, rankIndex));
                }
            }
            return piecesAfterMove;            
        }

        private static Piece ApplyMove(Square from, Square to, Square enPassantHit, Piece[,] pieces, Square current)
        {
            if (current.Equals(from) || current.Equals(enPassantHit))
            {
                return null;
            }
            if (current.Equals(to))
            {
                return from.SelectPiece(pieces).ApplyMove(from, to);
            }
            return current.SelectPiece(pieces)?.RemainInPlace();
        }

        public static Square Parse([NotNull] [StringLength(2, 2)] string square)
        {
            return new Square(ParseFileIndex(square[0]), ParseRankIndex(square[1]));
        }

        private static int ParseFileIndex(char file)
        {
            for (int index = 0; index < _Files.Length; index++)
            {
                if (_Files[index] == file)
                {
                    return index;
                }
            }
            throw NewInvalidFileException(file);
        }

        private static int ParseRankIndex(char rank)
        {
            try
            {
                return int.Parse(rank.ToString()) - 1;
            }
            catch
            {
                throw NewInvalidRankException(rank);
            }            
        }

        private static Exception NewInvalidFileException(char file)
        {
            var messageFormat = ExceptionMessages.Square_InvalidFile;
            var message = string.Format(messageFormat, file);
            return new ArgumentOutOfRangeException(nameof(file), message);
        }

        private static Exception NewInvalidRankException(char rank)
        {
            var messageFormat = ExceptionMessages.Square_InvalidRank;
            var message = string.Format(messageFormat, rank);
            return new ArgumentOutOfRangeException(nameof(rank), message);
        }
    }
}

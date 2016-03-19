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

        internal Square([Range(0, ChessBoard.Size - 1)] int fileIndex, [Range(0, ChessBoard.Size - 1)] int rankIndex)
        {
            FileIndex = fileIndex;
            RankIndex = rankIndex;
        }

        public char File
        {
            get { return _Files[FileIndex]; }
        }

        internal int FileIndex
        {
            get;
        }

        public int Rank
        {
            get { return RankIndex + 1; }
        }

        internal int RankIndex
        {
            get;
        }

        public override string ToString()
        {
            return $"{File}{Rank}";
        }

        internal Square Add(int fileSteps, int rankSteps)
        {
            return new Square(FileIndex + fileSteps, RankIndex + rankSteps);
        }

        internal bool TryAdd(int fileSteps, int rankSteps, out Square result)
        {
            int newFileIndex = FileIndex + fileSteps;

            if (IsValidIndex(newFileIndex))
            {
                int newRankIndex = RankIndex + rankSteps;

                if (IsValidIndex(newRankIndex))
                {
                    result = new Square(newFileIndex, newRankIndex);
                    return true;
                }
            }
            result = null;
            return false;
        }

        private static bool IsValidIndex(int index)
        {
            return 0 <= index && index < ChessBoard.Size;
        }            

        #region [====== Paths ======]

        internal IEnumerable<Square> CreateVerticalPath(int rankSteps)
        {
            foreach (var step in StepsInPath(rankSteps))
            {
                yield return new Square(FileIndex, RankIndex - step);
            }
        }

        internal IEnumerable<Square> CreateHorizontalPath(int fileSteps)
        {
            foreach (var step in StepsInPath(fileSteps))
            {
                yield return new Square(FileIndex - step, RankIndex);
            }
        }

        internal IEnumerable<Square> CreateCrossPath(int fileSteps, int rankSteps)
        {
            foreach (var step in StepsInPath(fileSteps, rankSteps))
            {
                yield return new Square(FileIndex - step.Item1, RankIndex - step.Item2);
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

        #endregion        

        #region [====== Parse ======]

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

        #endregion
    }
}

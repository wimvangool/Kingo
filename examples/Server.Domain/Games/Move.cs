using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Move
    {
        public readonly Square To;
        public readonly int FileSteps;
        public readonly int RankSteps;

        public Move(Square to, int fileSteps, int rankSteps)
        {
            To = to;
            FileSteps = fileSteps;
            RankSteps = rankSteps;
        }

        public bool IsStraightPath
        {
            get { return FileSteps == 0 || RankSteps == 0; }
        }

        public bool IsCrossPath
        {
            get { return Math.Abs(FileSteps) == Math.Abs(RankSteps); }
        }

        public override string ToString()
        {
            return $"[{FileSteps}, {RankSteps}]";
        }

        public bool IsEmptyPath(ChessBoard board)
        {
            IEnumerable<Square> path;

            if (TryGetSquaresInStraightLine(out path))
            {
                return path.All(board.IsEmpty);
            }
            return true;
        }

        private bool TryGetSquaresInStraightLine(out IEnumerable<Square> path)
        {            
            if (FileSteps == 0)
            {
                path = To.CreateVerticalPath(RankSteps);
                return true;
            }
            if (RankSteps == 0)
            {
                path = To.CreateHorizontalPath(FileSteps);
                return true;
            }
            if (IsCrossPath)
            {
                path = To.CreateCrossPath(FileSteps, RankSteps);
                return true;
            }
            path = null;
            return false;
        }        
    }
}

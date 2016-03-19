using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Move : ValueObject<Move>
    {        
        private Move(Square from, int fileSteps, int rankSteps)
        {
            From = from;
            FileSteps = fileSteps;
            RankSteps = rankSteps;
        }

        public Square From
        {
            get;
        }

        public Square To
        {
            get { return From.Add(FileSteps, RankSteps); }
        }

        public int FileSteps
        {
            get;
        }

        public int RankSteps
        {
            get;
        }

        public override string ToString()
        {
            return $"[{From}, {To}] ({FileSteps}, {RankSteps})";
        }

        public bool IsStraightPath
        {
            get { return FileSteps == 0 || RankSteps == 0; }
        }

        public bool IsCrossPath
        {
            get { return Math.Abs(FileSteps) == Math.Abs(RankSteps); }
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

        public bool TryGetSquaresInStraightLine(out IEnumerable<Square> path)
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
        
        public static Move Calculate(Square from, Square to)
        {
            if (to.Equals(from))
            {
                throw NewInvalidMoveException(to);
            }
            return new Move(from, to.FileIndex - from.FileIndex, to.RankIndex - from.RankIndex);
        }

        private static Exception NewInvalidMoveException(Square to)
        {
            var messageFormat = ExceptionMessages.Game_InvalidMoveSpecified;
            var message = string.Format(messageFormat, to);
            return new ArgumentException(message, nameof(to));
        }
    }
}

using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// This class contains factory methods to create objects purely for syntactical reasons.
    /// </summary>
    public static class New
    {        
        internal static Exception NegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.New_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        
    }
}

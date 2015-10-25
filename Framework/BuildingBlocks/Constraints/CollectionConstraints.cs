using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {        
        internal static Exception NewNegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.CollectionConstraints_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}

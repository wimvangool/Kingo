using System;
using System.Globalization;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class CollectionConstraints
    {
        internal static string NameOfElementAt(string member, object keyOrIndex)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", member, keyOrIndex);
        }

        internal static Exception NewNegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.CollectionConstraints_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }
    }
}

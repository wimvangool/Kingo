using System;
using Kingo.Constraints;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class IsValidSquareConstraint : Constraint<string>
    {
        protected override StringTemplate ErrorMessageIfNotSpecified => StringTemplate.Parse("'{member.Value}' is not a valid position identifier.");        

        public override bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length == 2)
            {
                return IsValidColumn(value[0]) && IsValidRow(value[1]);
            }
            return false;
        }

        private static bool IsValidColumn(char column)
        {
            return 'a'.CompareTo(column) <= 0 && column.CompareTo('h') <= 0;
        }

        private static bool IsValidRow(char row)
        {
            int rowValue;

            if (int.TryParse(row.ToString(), out rowValue))
            {
                return 1 <= rowValue && rowValue <= 8;
            }
            return false;
        }
    }

    internal static class SquareConstraints
    {
        public static IMemberConstraintBuilder<TMessage, string> IsValidSquare<TMessage>(this IMemberConstraintBuilder<TMessage, string> builder)
        {
            return builder.Apply(new IsValidSquareConstraint());
        }
    }
}

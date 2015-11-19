using System;

namespace Kingo.BuildingBlocks.Constraints
{
    public static partial class CollectionConstraints
    {
        private static readonly Identifier _Length = Identifier.Parse("Length");

        /// <summary>
        /// Returns the length of the specified array.
        /// </summary>   
        /// <param name="member">A member.</param>                     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>                  
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>        
        public static IMemberConstraintBuilder<T, int> Length<T, TValue>(this IMemberConstraintBuilder<T, TValue[]> member)
        {
            return member.Apply(instance => new DelegateFilter<T, TValue[], int>(instance, ArrayLength)).And((instance, length) => length, _Length);
        }

        private static int ArrayLength<T, TValue>(T instance, TValue[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.Length;
        }
    }
}

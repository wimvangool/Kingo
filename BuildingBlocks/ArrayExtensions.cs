using System;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Contains extension-methods for arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Creates and returns a new array containing all the elements of the source plus the element to add at the last index.
        /// </summary>
        /// <typeparam name="TElement">Type of the elements in the array.</typeparam>
        /// <param name="elements">An array of elements.</param>
        /// <param name="element">The element to add at the end of the new array.</param>
        /// <returns>A new array with the element at the last index.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="elements"/> is <c>null</c>.
        /// </exception>
        public static TElement[] Add<TElement>(this TElement[] elements, TElement element)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }
            var elementsPlusOne = new TElement[elements.Length + 1];

            Array.Copy(elements, 0, elementsPlusOne, 0, elements.Length);
            
            elementsPlusOne[elements.Length] = element;

            return elementsPlusOne;
        }
    }
}

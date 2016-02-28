using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kingo.DynamicMethods
{
    /// <summary>
    /// When implemented by a class, represents a filter of fields and properties that are to be
    /// used by a dynamic method's implementation.
    /// </summary>
    public interface IMemberFilter
    {
        /// <summary>
        /// Filters all specified <paramref name="fields"/>, returning only those fields that are to be used.
        /// </summary>
        /// <param name="fields">The fields to filter.</param>
        /// <returns>A collection of fields that may be used.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fields"/> is <c>null</c>.
        /// </exception>
        IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields);

        /// <summary>
        /// Filters all specified <paramref name="properties"/>, returning only those properties that are to be used.
        /// </summary>
        /// <param name="properties">The properties to filter.</param>
        /// <returns>A collection of properties that may be used.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="properties"/> is <c>null</c>.
        /// </exception>
        IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Constraints
{
    /// <summary>
    /// Represents a constraint over a certain value that is associated with a specific error message.
    /// </summary>
    public interface IConstraintWithErrorMessage
    {
        /// <summary>
        /// Formats the error message associated with this constraint and returns the result.
        /// </summary>
        /// <param name="formatProvider">A <see cref="IFormatProvider" /> that is used for placeholders that define a specific format.</param>
        /// <returns>The formatted error message.</returns>
        StringTemplate FormatErrorMessage(IFormatProvider formatProvider = null);
    }
}

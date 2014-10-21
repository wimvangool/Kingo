using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Messaging.Validation
{
    /// <summary>
    /// Represents a value that can be used on the <see cref="RequiredAttribute" /> to indicate
    /// what values of a string are interpreted as invalid.
    /// </summary>
    public enum RequiredStringConstraint
    {
        /// <summary>
        /// Indicates that a string may not be <c>null</c>.
        /// </summary>
        NotNull,

        /// <summary>
        /// Indicates that a string may not be <c>null</c> or the <see cref="string.Empty">empty string</see>.
        /// </summary>
        NotNullOrEmpty,

        /// <summary>
        /// Indicates that a string may not be <c>null</c>, the <see cref="string.Empty">empty string</see>,
        /// or contain only white space.
        /// </summary>
        NotNullOrWhiteSpace
    }
}

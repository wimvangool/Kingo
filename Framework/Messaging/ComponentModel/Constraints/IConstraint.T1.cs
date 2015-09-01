using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace ServiceComponents.ComponentModel.Constraints
{
    /// <summary>
    /// Represents a constraint over a certain <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">Type of the value this constraint is for.</typeparam>
    public interface IConstraint<in TValue>
    {        
        /// <summary>
        /// Determines whether or not this constraint is satisfied by the value obtained from the specified <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="valueFactory">Delegate used to obtain the value to check this constraint against.</param>
        /// <returns>
        /// <c>true</c> if this constraint was satisfied by the value obtained from the specified <paramref name="valueFactory"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        bool IsSatisfiedBy(Func<TValue> valueFactory);

        /// <summary>
        /// Determines whether or not this constraint is satisfied by the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>
        /// <c>true</c> if this constraint was satisfied by the specified <paramref name="value"/>; otherwise <c>false</c>.
        /// </returns>
        bool IsSatisfiedBy(TValue value);

        /// <summary>
        /// Creates and returns a string-representation of this constraint using the specified <paramref name="memberName"/> as the variable name.
        /// </summary>
        /// <param name="memberName">Name of the variable to use in the string-representation.</param>
        /// <returns>A string-representation of this constraint.</returns>
        string ToString(string memberName);
    }
}

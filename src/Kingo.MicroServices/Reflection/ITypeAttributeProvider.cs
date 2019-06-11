using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.Reflection
{
    /// <summary>
    /// When implemented by a class, serves as a provider of attributes declared on a class or struct.
    /// </summary>
    public interface ITypeAttributeProvider : IAttributeProvider
    {
        /// <summary>
        /// The type for which the attributes are provided.
        /// </summary>
        Type Type
        {
            get;
        }
    }
}

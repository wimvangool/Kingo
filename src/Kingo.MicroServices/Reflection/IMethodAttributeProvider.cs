using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kingo.Reflection
{
    /// <summary>
    /// When implemented by a class, serves as a provider of attributes declared on a method.
    /// </summary>
    public interface IMethodAttributeProvider : IAttributeProvider
    {
        /// <summary>
        /// The method for which the attributes are provided.
        /// </summary>
        MethodInfo Info
        {
            get;
        }
    }
}

using System;
using System.Reflection;

namespace Kingo.Reflection
{
    /// <summary>
    /// When implemented by a class, represents a provider of attributes that are specified on a specific parameter.
    /// </summary>
    public interface IParameterAttributeProvider : IAttributeProvider
    {
        /// <summary>
        /// Returns the parameter type.
        /// </summary>
        Type Type
        {
            get;
        }

        /// <summary>
        /// Returns the parameter info.
        /// </summary>
        ParameterInfo Info
        {
            get;
        }
    }
}

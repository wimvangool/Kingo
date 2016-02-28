using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kingo.DynamicMethods
{
    /// <summary>
    /// When implemented, this attribute can be used to decorate a class or struct that has one of its
    /// core methods implemented through a dynamic method. When applied, the attribute is used to select
    /// the fields and properties from the type that will be used in the method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public abstract class MemberFilterAttribute : Attribute, IMemberFilter
    {
        /// <inheritdoc />
        public abstract IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields);

        /// <inheritdoc />
        public abstract IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties);
    }
}

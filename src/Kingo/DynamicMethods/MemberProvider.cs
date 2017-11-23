using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.DynamicMethods
{
    internal class MemberProvider : IMemberFilter
    {
        #region [====== Instance Members ======]

        private const BindingFlags _MemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
        private readonly Type _type;

        protected MemberProvider(Type type)
        {
            _type = type;
        }

        public IEnumerable<FieldInfo> Fields =>
             Filter(FieldsOf(_type));

        private static IEnumerable<FieldInfo> FieldsOf(IReflect type) =>
             type.GetFields(_MemberFlags);

        public IEnumerable<PropertyInfo> Properties =>
             Filter(PropertiesOf(_type));

        private static IEnumerable<PropertyInfo> PropertiesOf(IReflect type) =>
             type.GetProperties(_MemberFlags);

        public virtual IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields) =>
             fields;

        public virtual IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties) =>
             Enumerable.Empty<PropertyInfo>();

        #endregion

        #region [====== Static Members ======]

        public static MemberProvider For(Type type)
        {
            MemberProvider memberProvider;

            if (CustomMemberProvider.HasCustomFilter(type, out memberProvider))
            {
                return memberProvider;
            }
            if (DataMemberProvider.IsDataContract(type, out memberProvider))
            {
                return memberProvider;
            }
            if (SerializableFieldProvider.IsSerializable(type, out memberProvider))
            {
                return memberProvider;
            }
            return new MemberProvider(type);
        }

        protected static bool HasAttribute(MemberInfo typeOrMember, Type attributeType) =>
             typeOrMember.GetCustomAttributes(attributeType).Any();

        #endregion
    }
}

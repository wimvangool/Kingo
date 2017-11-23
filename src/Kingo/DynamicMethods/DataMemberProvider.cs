using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Kingo.DynamicMethods
{
    internal sealed class DataMemberProvider : MemberProvider
    {
        #region [====== Instance Members ======]        

        private DataMemberProvider(Type type)
            : base(type) { }

        public override IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields) =>
             fields.Where(IsDataMember);

        public override IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties) =>
             properties.Where(IsDataMember);

        private static bool IsDataMember(MemberInfo member) =>
             HasAttribute(member, typeof(DataMemberAttribute));

        #endregion

        #region [====== Static Members ======]

        internal static bool IsDataContract(Type type, out MemberProvider memberProvider)
        {
            if (HasAttribute(type, typeof(DataContractAttribute)))
            {
                memberProvider = new DataMemberProvider(type);
                return true;
            }
            memberProvider = null;
            return false;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.DynamicMethods
{
    internal sealed class CustomMemberProvider : MemberProvider
    {
        #region [====== Instance Members ======]

        private readonly IMemberFilter _memberFilter;

        private CustomMemberProvider(Type type, IMemberFilter memberFilter)
            : base(type)
        {
            _memberFilter = memberFilter;
        }

        public override IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields) =>
             _memberFilter.Filter(fields);

        public override IEnumerable<PropertyInfo> Filter(IEnumerable<PropertyInfo> properties) =>
             _memberFilter.Filter(properties);

        #endregion

        #region [====== Static Members ======]

        internal static bool HasCustomFilter(Type type, out MemberProvider memberProvider)
        {
            var memberFilter = type.GetCustomAttributes().OfType<IMemberFilter>().FirstOrDefault();
            if (memberFilter != null)
            {
                memberProvider = new CustomMemberProvider(type, memberFilter);
                return true;
            }
            memberProvider = null;
            return false;
        }

        #endregion
    }
}

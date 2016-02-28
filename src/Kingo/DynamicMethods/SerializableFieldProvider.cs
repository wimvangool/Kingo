using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.DynamicMethods
{
    internal sealed class SerializableFieldProvider : MemberProvider
    {
        #region [====== Instance Members ======]        

        private SerializableFieldProvider(Type type)
            : base(type) { }

        public override IEnumerable<FieldInfo> Filter(IEnumerable<FieldInfo> fields)
        {
            return fields.Where(IsSerialized);
        }        

        private static bool IsSerialized(MemberInfo member)
        {
            return !HasAttribute(member, typeof(NonSerializedAttribute));
        }

        #endregion

        #region [====== Static Members ======]

        internal static bool IsSerializable(Type type, out MemberProvider memberProvider)
        {
            if (HasAttribute(type, typeof(SerializableAttribute)))
            {
                memberProvider = new SerializableFieldProvider(type);
                return true;
            }
            memberProvider = null;
            return false;
        }

        #endregion
    }
}

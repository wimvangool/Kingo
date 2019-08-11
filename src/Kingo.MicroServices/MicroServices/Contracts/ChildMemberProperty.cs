using System;
using System.Reflection;

namespace Kingo.MicroServices.Contracts
{
    internal sealed class ChildMemberProperty : ChildMember<PropertyInfo>
    {
        public ChildMemberProperty(PropertyInfo property, ChildMemberAttribute attribute) :
            base(property, attribute) { }

        public override Type Type =>
            Member.PropertyType;

        protected override object GetValue(object instance) =>
            Member.GetValue(instance);
    }
}

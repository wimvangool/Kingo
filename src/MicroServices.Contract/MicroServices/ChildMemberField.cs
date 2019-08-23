using System;
using System.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ChildMemberField : ChildMember<FieldInfo>
    {        
        public ChildMemberField(FieldInfo field, ChildMemberAttribute attribute) :
            base(field, attribute) { }

        public override Type Type =>
            Member.FieldType;

        protected override object GetValue(object instance) =>
            Member.GetValue(instance);
    }
}

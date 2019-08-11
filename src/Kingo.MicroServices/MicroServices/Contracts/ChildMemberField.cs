using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Kingo.MicroServices.Contracts
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

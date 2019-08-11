using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

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

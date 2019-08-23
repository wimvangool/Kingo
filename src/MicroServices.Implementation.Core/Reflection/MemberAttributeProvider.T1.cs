using System;
using System.Linq;
using System.Reflection;

namespace Kingo.Reflection
{
    internal sealed class MemberAttributeProvider<TMember> : AttributeProvider
        where TMember : MemberInfo
    {        
        public MemberAttributeProvider(TMember member)
        {
            Member = member;
        }

        protected override object Target =>
            Member;

        protected override string TargetName =>
            Member.Name;

        public TMember Member
        {
            get;
        }

        protected override Attribute[] LoadAttributes() =>
            Member.GetCustomAttributes(true).OfType<Attribute>().ToArray();
    }
}

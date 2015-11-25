using System;
using System.Text;

namespace Kingo.Constraints
{
    internal sealed class EmptyStack : MemberNameComponentStack
    {
        private readonly Type _instanceType;

        internal EmptyStack(Type instanceType)
        {
            _instanceType = instanceType;
        }

        internal override Type InstanceType
        {
            get { return _instanceType; }
        }

        internal override string Top
        {
            get { return _instanceType.Name; }
        }

        internal override MemberNameComponentStack Push(Identifier identifier)
        {
            return new IdentifierComponent(_instanceType, identifier, null);
        }

        internal override MemberNameComponentStack Push(IndexList indexList)
        {
            return new IndexListComponent(_instanceType, indexList, null);
        }

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            memberName = null;
            return false;
        }

        internal override void WriteTo(StringBuilder fullName, bool displayName)
        {
            if (displayName)
            {
                fullName.Append(InstanceName);
            }
        }
    }
}

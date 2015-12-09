using System;
using System.Text;

namespace Kingo.Constraints
{
    internal sealed class IdentifierComponent : MemberNameComponentStack
    {
        private readonly Type _instanceType;
        private readonly Identifier _identifier;
        private readonly MemberNameComponentStack _bottomStack;      

        internal IdentifierComponent(Type instanceType, Identifier identifier, MemberNameComponentStack bottomStack)
        {            
            _instanceType = instanceType;
            _identifier = identifier;
            _bottomStack = bottomStack;
        }

        internal override Type InstanceType
        {
            get { return _instanceType; }
        }

        internal override string Top
        {
            get { return _identifier.ToString(); }
        }        

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            memberName = _bottomStack ?? new EmptyStack(_instanceType);
            return true;
        }

        internal override void WriteTo(StringBuilder fullName, bool displayName)
        {
            if (_bottomStack == null)
            {
                fullName.Append(Top);
            }
            else
            {
                _bottomStack.WriteTo(fullName, displayName);

                fullName.Append('.');
                fullName.Append(Top);
            }
        }
    }
}

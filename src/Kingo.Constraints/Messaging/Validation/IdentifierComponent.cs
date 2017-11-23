using System;
using System.Text;

namespace Kingo.Messaging.Validation
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

        internal override Type InstanceType =>
             _instanceType;

        internal override string Top =>
             _identifier.ToString();

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

using System;
using System.Text;

namespace Kingo.Messaging.Validation
{
    internal sealed class IndexListComponent : MemberNameComponentStack
    {
        private readonly Type _instanceType;
        private readonly IndexList _indexList;
        private readonly MemberNameComponentStack _bottomStack;        

        internal IndexListComponent(Type instanceType, IndexList indexList, MemberNameComponentStack bottomStack)
        {
            _instanceType = instanceType;
            _indexList = indexList;
            _bottomStack = bottomStack;
        }

        internal override Type InstanceType => _instanceType;

        internal override string Top => _indexList.ToString();

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            memberName = _bottomStack ?? new EmptyStack(_instanceType);
            return true;
        }

        internal override void WriteTo(StringBuilder fullName, bool displayName)
        {
            if (_bottomStack == null && displayName)
            {                
                fullName.Append(RemoveEmptyIndexerBrackets(InstanceName));
            }
            else if (_bottomStack != null)
            {
                _bottomStack.WriteTo(fullName, displayName);
            }
            fullName.Append(Top);
        }

        private static string RemoveEmptyIndexerBrackets(string instanceName)
        {
            var openBracketIndex = instanceName.IndexOf('[');
            if (openBracketIndex >= 0)
            {
                return instanceName.Substring(0, openBracketIndex);
            }
            return instanceName;
        }
    }
}

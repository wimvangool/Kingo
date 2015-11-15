using System;
using System.Text;

namespace Kingo.BuildingBlocks.Constraints
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

        internal override Type InstanceType
        {
            get { return _instanceType; }
        }

        internal override string Top
        {
            get { return _indexList.ToString(); }
        }       

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            memberName = _bottomStack ?? new EmptyStack(_instanceType);
            return true;
        }

        internal override void WriteTo(StringBuilder fullName, bool displayName)
        {
            if (_bottomStack == null && displayName)
            {
                var instanceName = InstanceName;
                if (instanceName.EndsWith("[]"))
                {
                    instanceName = InstanceName.Substring(0, instanceName.Length - 2);
                }
                fullName.Append(instanceName);
            }
            else if (_bottomStack != null)
            {
                _bottomStack.WriteTo(fullName, displayName);
            }
            fullName.Append(Top);
        }
    }
}

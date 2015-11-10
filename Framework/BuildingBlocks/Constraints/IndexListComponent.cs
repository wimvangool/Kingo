using System.Text;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class IndexListComponent : MemberNameComponentStack
    {
        private readonly IndexList _indexList;
        private readonly MemberNameComponentStack _bottomStack;        

        internal IndexListComponent(IndexList indexList, MemberNameComponentStack bottomStack)
        {
            _indexList = indexList;
            _bottomStack = bottomStack;
        }

        internal override string Top
        {
            get { return _indexList.ToString(); }
        }       

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            return (memberName = _bottomStack) != null;
        }

        internal override void WriteTo(StringBuilder fullName)
        {
            if (_bottomStack != null)
            {
                _bottomStack.WriteTo(fullName);
            }
            fullName.Append(Top);
        }
    }
}

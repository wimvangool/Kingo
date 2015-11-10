using System.Text;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class IdentifierComponent : MemberNameComponentStack
    {
        private readonly Identifier _identifier;
        private readonly MemberNameComponentStack _bottomStack;      

        internal IdentifierComponent(Identifier identifier, MemberNameComponentStack bottomStack)
        {
            _identifier = identifier;
            _bottomStack = bottomStack;
        }

        internal override string Top
        {
            get { return _identifier.ToString(); }
        }        

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            return (memberName = _bottomStack) != null;
        }

        internal override void WriteTo(StringBuilder fullName)
        {
            if (_bottomStack == null)
            {
                fullName.Append(Top);
            }
            else
            {
                _bottomStack.WriteTo(fullName);

                fullName.Append('.');
                fullName.Append(Top);
            }
        }
    }
}

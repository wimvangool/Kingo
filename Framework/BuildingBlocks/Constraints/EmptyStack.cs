using System.Text;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class EmptyStack : MemberNameComponentStack
    {
        internal override string Top
        {
            get { return string.Empty; }
        }

        internal override MemberNameComponentStack Push(Identifier identifier)
        {
            return new IdentifierComponent(identifier, null);
        }

        internal override MemberNameComponentStack Push(IndexList indexList)
        {
            return new IndexListComponent(indexList, null);
        }      

        internal override bool Pop(out MemberNameComponentStack memberName)
        {
            memberName = null;
            return false;
        }

        internal override void WriteTo(StringBuilder fullName) { }
    }
}

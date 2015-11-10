using System.Text;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class MemberNameComponentStack
    {
        internal abstract string Top
        {
            get;
        }

        internal virtual MemberNameComponentStack Push(Identifier identifier)
        {
            return new IdentifierComponent(identifier, this);
        }

        internal virtual MemberNameComponentStack Push(IndexList indexList)
        {
            return new IndexListComponent(indexList, this);
        }        

        internal abstract bool Pop(out MemberNameComponentStack memberName);        

        public override string ToString()
        {
            var fullName = new StringBuilder(); 

            WriteTo(fullName);

            return fullName.ToString();
        }

        internal abstract void WriteTo(StringBuilder fullName);        
    }
}

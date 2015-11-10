using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class Member : IMember
    {
        internal readonly MemberNameComponentStack NameComponentStack;

        protected Member(MemberNameComponentStack nameComponentStack)
        {
            NameComponentStack = nameComponentStack;
        }        

        public string FullName
        {
            get { return NameComponentStack.ToString(); }
        }        
        
        public string Name
        {
            get { return NameComponentStack.Top; }
        }        

        public abstract Type Type
        {
            get;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", FullName, Type);
        }        
    }
}

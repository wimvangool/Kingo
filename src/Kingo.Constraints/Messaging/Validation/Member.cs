using System;

namespace Kingo.Messaging.Validation
{
    internal abstract class Member : IMember
    {              
        public string DisplayName
        {
            get { return NameComponentStack.ToString(true); }
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
            return DisplayName;
        }

        internal abstract MemberNameComponentStack NameComponentStack
        {
            get;
        }
    }
}

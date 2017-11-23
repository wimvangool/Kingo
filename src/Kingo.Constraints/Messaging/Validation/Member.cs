using System;

namespace Kingo.Messaging.Validation
{
    internal abstract class Member : IMember
    {              
        public string DisplayName => NameComponentStack.ToString(true);

        public string FullName => NameComponentStack.ToString();

        public string Name => NameComponentStack.Top;

        public abstract Type Type
        {
            get;
        }

        public override string ToString() => DisplayName;

        internal abstract MemberNameComponentStack NameComponentStack
        {
            get;
        }
    }
}

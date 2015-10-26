using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class Member : IMember
    {
        private const string _NameSeparator = ".";                     

        public string FullName
        {
            get
            {
                if (ParentNames.Length == 0)
                {
                    return Name;                    
                }
                return string.Join(_NameSeparator, ParentNames) + _NameSeparator + Name;
            }
        }

        protected abstract string[] ParentNames
        {
            get;
        }

        public abstract string Name
        {
            get;
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

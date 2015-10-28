using System;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class Member : IMember
    {
        private const string _NameSeparator = "."; 
        
        public string Key
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

        public string FullName
        {
            get
            {
                if (FieldsOrProperties.Length == 0)
                {
                    return Key;
                }
                return Key + _NameSeparator + string.Join(_NameSeparator, FieldsOrProperties.Select(identifier => identifier.ToString()));
            }
        }

        protected abstract string[] ParentNames
        {
            get;
        }

        protected abstract Identifier[] FieldsOrProperties
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
            return string.Format("Key = {0}, FullName = {1}, Type = {2}", Key, FullName, Type);
        }        
    }
}

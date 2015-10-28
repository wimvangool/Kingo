using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class Member : IMember
    {                
        public string Key
        {
            get
            {
                if (ParentNames.Length == 0)
                {
                    return Name;
                }
                return Join(ParentNames.Concat(new [] { Name }));
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
                return Join(ParentNames.Concat(new[] { Name }).Concat(FieldsOrProperties.Select(identifier => identifier.ToString())));                
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

        private const string _NameSeparator = "."; 

        internal static string Join(IEnumerable<string> names)
        {
            return string.Join(_NameSeparator, names);
        }
    }
}

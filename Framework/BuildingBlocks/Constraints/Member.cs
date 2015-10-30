using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal abstract class Member : IMember
    {
        private readonly Lazy<string> _key;
        private readonly Lazy<string> _fullName;

        protected Member()
        {
            _key = new Lazy<string>(CreateKey);
            _fullName = new Lazy<string>(CreateFullName);
        }

        public string Key
        {
            get { return _key.Value; }
        }

        private string CreateKey()
        {
            if (ParentNames.Length == 0)
            {
                return Name;
            }            
            return Join(ParentNames.Concat(new[] { Name }));
        }

        public string FullName
        {
            get { return _fullName.Value; }
        }

        private string CreateFullName()
        {
            if (FieldsOrProperties.Length == 0)
            {
                return Key;
            }
            return Join(ParentNames.Concat(new[] { Name }).Concat(FieldsOrProperties.Select(identifier => identifier.ToString()))); 
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
            return string.Join(_NameSeparator, names.Where(name => !string.IsNullOrEmpty(name)));
        }
    }
}

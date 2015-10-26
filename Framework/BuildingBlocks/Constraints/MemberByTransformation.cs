using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberByTransformation : Member
    {
        private readonly string[] _parentNames;
        private readonly string _name;
        private readonly Type _type;

        internal MemberByTransformation(string[] parentNames, string name, Type type)         
        {
            _parentNames = parentNames;
            _name = name;
            _type = type;
        }

        protected override string[] ParentNames
        {
            get { return _parentNames; }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override Type Type
        {
            get { return _type; }
        }

        internal MemberByTransformation Transform(Type newType, Func<string, string> nameSelector)
        {
            if (newType == _type && nameSelector == null)
            {
                return this;
            }
            return new MemberByTransformation(_parentNames, Rename(nameSelector), newType);
        }

        private string Rename(Func<string, string> nameSelector)
        {
            return nameSelector == null ? _name : nameSelector.Invoke(_name);
        }

        internal IMember WithValue(object value)
        {
            return new MemberWithValue(this, value);
        }
    }
}

using System;

namespace Kingo.BuildingBlocks.Constraints
{    
    internal sealed class MemberWithValue : IMember
    {
        private readonly IMember _member;
        private readonly object _value;

        internal MemberWithValue(IMember member, object value)
        {
            _member = member;
            _value = value;
        }                

        public string Key
        {
            get { return _member.Key; }    
        }
        
        public string FullName
        {
            get { return _member.FullName; }
        }
        
        public string Name
        {
            get { return _member.Name; }
        }
        
        public Type Type
        {
            get { return ReferenceEquals(_value, null) ? _member.Type : _value.GetType(); }
        }
        
        public object Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return _member.ToString();
        }          
    }
}

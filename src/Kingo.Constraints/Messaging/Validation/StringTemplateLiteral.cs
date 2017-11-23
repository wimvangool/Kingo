using System;
using Kingo.DynamicMethods;

namespace Kingo.Messaging.Validation
{
    internal sealed class StringTemplateLiteral : StringTemplateComponent, IEquatable<StringTemplateLiteral>
    {
        private readonly StringTemplateComponent _nextComponent;
        private readonly string _literal;

        internal StringTemplateLiteral(string literal, StringTemplateComponent nextComponent)
        {
            _nextComponent = nextComponent;
            _literal = literal;
        }

        internal override StringTemplateComponent NextComponent => _nextComponent;

        internal override StringTemplateComponent AttachLast(StringTemplateComponent nextComponent)
        {
            if (_nextComponent == null)
            {
                return new StringTemplateLiteral(_literal, nextComponent);
            }
            return new StringTemplateLiteral(_literal, _nextComponent.AttachLast(nextComponent));
        }

        internal override int CountLiterals() => _literal.Length + (_nextComponent == null ? 0 : _nextComponent.CountLiterals());

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj) => Equals(obj as StringTemplateLiteral);

        public bool Equals(StringTemplateLiteral other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _literal == other._literal && Equals(_nextComponent, other._nextComponent);
        }

        public override int GetHashCode() => GetHashCodeMethod.Invoke(_literal) ^ GetHashCodeMethod.Invoke(_nextComponent);

        #endregion        

        public override string ToString() => _literal;

        internal override StringTemplateComponent Format(Identifier identifier, object argument, IFormatProvider formatProvider)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }
            if (_nextComponent == null)
            {
                return this;
            }
            return new StringTemplateLiteral(_literal, _nextComponent.Format(identifier, argument, formatProvider));           
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    internal sealed class StringTemplateVariable : StringTemplateComponent, IEquatable<StringTemplateVariable>
    {
        private readonly StringTemplateComponent _nextComponent;
        private readonly Identifier _identifier;
        private readonly Identifier[] _expression;
        private readonly string _format;

        internal StringTemplateVariable(Identifier identifier, Identifier[] expression, string format, StringTemplateComponent nextComponent)
        {
            _nextComponent = nextComponent;
            _identifier = identifier;
            _expression = expression;
            _format = format;
        }

        internal override StringTemplateComponent NextComponent => _nextComponent;

        internal override StringTemplateComponent AttachLast(StringTemplateComponent nextComponent)
        {
            if (_nextComponent == null)
            {
                return new StringTemplateVariable(_identifier, _expression, _format, nextComponent);
            }
            return new StringTemplateVariable(_identifier, _expression, _format, _nextComponent.AttachLast(nextComponent));
        }

        internal override int CountLiterals() => _nextComponent == null ? 0 : _nextComponent.CountLiterals();

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj) => Equals(obj as StringTemplateVariable);

        public bool Equals(StringTemplateVariable other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return
                Equals(_identifier, other._identifier) &&
                Equals(_expression, other._expression) &&
                Equals(_format, other._format) &&
                Equals(_nextComponent, other._nextComponent);
        }

        private static bool Equals(IReadOnlyCollection<Identifier> left, IReadOnlyCollection<Identifier> right) => left.Count == right.Count && left.SequenceEqual(right);

        public override int GetHashCode() => _identifier.GetHashCode();

        #endregion

        public override string ToString()
        {
            var expression = string.Join(".", new[] { _identifier }.Concat(_expression));

            return string.IsNullOrEmpty(_format)
                ? '{' + expression + '}'
                : '{' + expression + ':' + _format + '}';
        }

        internal override StringTemplateComponent Format(Identifier identifier, object argument, IFormatProvider formatProvider)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }
            var nextComponent = _nextComponent == null ? null : _nextComponent.Format(identifier, argument, formatProvider);

            if (identifier == _identifier)
            {
                return new StringTemplateLiteral(Format(argument, _expression, _format, formatProvider), nextComponent);
            }
            return new StringTemplateVariable(_identifier, _expression, _format, nextComponent);
        }

        private static string Format(object argument, IReadOnlyList<Identifier> expression, string format, IFormatProvider formatProvider)
        {
            var instance = Evaluate(argument, expression);
            if (instance == null)
            {
                return StringTemplate.NullValue;
            }
            var formattable = instance as IFormattable;
            if (formattable == null)
            {
                return instance.ToString();
            }
            return formattable.ToString(format, formatProvider);
        }

        private static object Evaluate(object argument, IReadOnlyList<Identifier> expression)
        {
            for (int index = 0; index < expression.Count && argument != null; index++)
            {
                argument = Evaluate(argument, expression[index].ToString());
            }
            return argument;
        }

        private static object Evaluate(object argument, string identifier)
        {            
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                        
            var argumentType = argument.GetType();
            var property = argumentType.GetProperty(identifier, bindingFlags);
            if (property != null)
            {
                return property.GetValue(argument, null);
            }
            var field = argumentType.GetField(identifier, bindingFlags);
            if (field != null)
            {
                return field.GetValue(argument);
            }
            var explicitlyImplementedProperty = GetExplicitlyImplementedProperty(argumentType, identifier, bindingFlags);
            if (explicitlyImplementedProperty != null)
            {
                return explicitlyImplementedProperty.GetValue(argument, null);
            }
            throw NewMemberNotFoundException(argumentType, identifier);
        }

        private static PropertyInfo GetExplicitlyImplementedProperty(Type argumentType, string identifier, BindingFlags bindingFlags)
        {
            var properties =
                from interfaceType in argumentType.GetInterfaces()
                let property = interfaceType.GetProperty(identifier, bindingFlags)
                where property != null
                select property;

            return properties.FirstOrDefault();
        }

        private static Exception NewMemberNotFoundException(Type argumentType, string identifier)
        {
            var messageFormat = ExceptionMessages.StringTemplateVariable_MemberNotFound;
            var message = string.Format(messageFormat, argumentType, identifier);
            return new ArgumentException(message);
        }
    }
}

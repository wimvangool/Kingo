using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Represents a format string that can contain placeholder values that are identified by an instance name.
    /// </summary>
    public sealed class StringTemplate : IEquatable<StringTemplate>
    {
        /// <summary>
        /// The value that is used in templates to represent the <c>null</c>-value.
        /// </summary>
        public const string NullValue = "<null>";

        private readonly StringTemplateComponent _template;

        private StringTemplate(StringTemplateComponent template)
        {
            _template = template;
        }

        /// <summary>
        /// Returns the amount of literal characters in this template.
        /// </summary>
        public int LiteralCount
        {
            get { return _template.CountLiterals(); }
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as StringTemplate);
        }

        /// <inheritdoc />
        public bool Equals(StringTemplate other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _template.Equals(other._template);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _template.GetHashCode();
        }

        #endregion

        #region [====== Format & ToString ======]

        /// <summary>
        /// Replaces all placeholders of the specified <paramref name="identifier"/> with the value of <paramref name="argument"/>.
        /// </summary>
        /// <param name="identifier">Identifier of the placeholder.</param>
        /// <param name="argument">Value of the placeholder.</param>
        /// <param name="formatProvider">A <see cref="IFormatProvider" /> that is used for placeholders that define a specific format.</param>
        /// <returns>The formatted string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="identifier"/> is <c>null</c>.
        /// </exception>
        public StringTemplate Format(string identifier, object argument, IFormatProvider formatProvider = null)
        {
            return new StringTemplate(_template.Format(identifier, argument, formatProvider ?? CultureInfo.CurrentCulture));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _template.ToString(new StringBuilder());
        }

        #endregion

        #region [====== Concat ======]

        /// <summary>
        /// Concatenates this template with another template.
        /// </summary>
        /// <param name="template">The template to concatenate.</param>
        /// <returns>The concatenated template.</returns>
        public StringTemplate Concat(string template)
        {
            return Concat(Parse(template));
        }

        /// <summary>
        /// Concatenates this template with another template.
        /// </summary>
        /// <param name="template">The template to concatenate.</param>
        /// <returns>The concatenated template.</returns>
        public StringTemplate Concat(StringTemplate template)
        {
            if (template == null || template.LiteralCount == 0)
            {
                return this;
            }
            return new StringTemplate(_template.AttachLast(template._template));
        }

        /// <summary>
        /// Returns the concatenation of <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        /// <param name="left">Left template.</param>
        /// <param name="right">Right template.</param>
        /// <returns>The concatenated template.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="right"/> is not in a correct format.
        /// </exception>
        public static StringTemplate operator +(StringTemplate left, string right)
        {
            return left + Parse(right);
        }

        /// <summary>
        /// Returns the concatenation of <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        /// <param name="left">Left template.</param>
        /// <param name="right">Right template.</param>
        /// <returns>The concatenated template.</returns>
        public static StringTemplate operator +(StringTemplate left, StringTemplate right)
        {
            if (left == null)
            {
                return right;
            }            
            return left.Concat(right);
        }

        #endregion

        #region [====== Parse ======]

        private const char _BraceOpen = '{';
        private const char _BraceClose = '}';

        /// <summary>
        /// Parses the specified <paramref name="templateFormat"/> such that it can be formatted using arbitrary arguments.
        /// </summary>
        /// <param name="templateFormat">The format string to parse.</param>
        /// <returns><c>null</c> if <paramref name="templateFormat"/> is <c>null</c>; otherwise a parsed format string.</returns>        
        /// <exception cref="ArgumentException">
        /// <paramref name="templateFormat"/> is not in a correct format.
        /// </exception>
        public static StringTemplate Parse(string templateFormat)
        {
            if (templateFormat == null)
            {
                return null;
            }                        
            return new StringTemplate(ParseNextComponent(templateFormat));
        }

        private static StringTemplateComponent ParseNextComponent(string templateFormat)
        {
            return templateFormat.Length == 0
                ? new StringTemplateLiteral(string.Empty, null)
                : ParseNextComponent(templateFormat, 0);
        }

        private static StringTemplateComponent ParseNextComponent(string templateFormat, int index)
        {                        
            var literal = new StringBuilder(templateFormat.Length - index);

            while (index < templateFormat.Length)
            {
                var character = templateFormat[index];
                if (character == _BraceOpen)
                {
                    char lookAheadCharacter;

                    if (TryLookAhead(templateFormat, index + 1, out lookAheadCharacter))
                    {
                        if (lookAheadCharacter == _BraceOpen)
                        {
                            literal.Append(_BraceOpen);
                            index += 2;
                            continue;
                        }
                        var nextComponent = ParseVariableComponent(templateFormat, index + 1);

                        if (literal.Length == 0)
                        {
                            return nextComponent;
                        }
                        return new StringTemplateLiteral(literal.ToString(), nextComponent);
                    }
                    throw NewUnexpectedCharacterException(templateFormat, index, character);
                }
                if (character == _BraceClose)
                {
                    char lookAheadCharacter;

                    if (TryLookAhead(templateFormat, index + 1, out lookAheadCharacter))
                    {
                        if (lookAheadCharacter == _BraceClose)
                        {
                            literal.Append(_BraceClose);
                            index += 2;
                            continue;
                        }
                    }
                    throw NewUnexpectedCharacterException(templateFormat, index, character);
                }
                literal.Append(character);
                index++;
            }
            if (literal.Length == 0)
            {
                return null;
            }
            return new StringTemplateLiteral(literal.ToString(), null);
        }        

        private static bool TryLookAhead(string templateFormat, int index, out char lookAheadCharacter)
        {
            if (index < templateFormat.Length)
            {
                lookAheadCharacter = templateFormat[index];
                return true;
            }
            lookAheadCharacter = '\0';
            return false;
        }

        private static StringTemplateComponent ParseVariableComponent(string templateFormat, int index)
        {
            var variableStartIndex = index;
            var variableEndIndex = index;

            while (variableEndIndex < templateFormat.Length)
            {
                var character = templateFormat[variableEndIndex];
                if (character == _BraceOpen)
                {
                    throw NewUnexpectedCharacterException(templateFormat, index, character);
                }
                if (character == _BraceClose)
                {
                    var nextComponent = ParseNextComponent(templateFormat, variableEndIndex + 1);
                    var variableLength = variableEndIndex - variableStartIndex;

                    return ParseVariableComponent(templateFormat, variableStartIndex, variableLength, nextComponent);
                }                
                variableEndIndex++;
            }
            throw NewMissingClosingBraceException(templateFormat, templateFormat.Length - 1);
        }     
   
        private static StringTemplateComponent ParseVariableComponent(string templateFormat, int variableStartIndex, int variableLength, StringTemplateComponent nextComponent)
        {
            if (variableLength == 0)
            {
                throw NewMissingIdentifierException(templateFormat, variableStartIndex);
            }
            string format = null;

            var identifierExpected = true;
            var identifierList = new LinkedList<StringBuilder>();
            var identifier = new StringBuilder(variableLength);
            var index = 0;

            while (index < variableLength)
            {
                var characterIndex = variableStartIndex + index;
                var character = templateFormat[characterIndex];

                if (character == '.')
                {
                    if (identifier.Length == 0)
                    {
                        throw NewMissingIdentifierException(templateFormat, characterIndex);
                    }
                    identifierList.AddLast(identifier);
                    identifier = new StringBuilder(variableLength - index);
                    identifierExpected = true;
                }
                else if (character == ':')
                {                    
                    format = templateFormat.Substring(characterIndex + 1, variableLength - index - 1);
                    break;
                }
                else if (char.IsLetter(character) || character == '_')
                {
                    identifierExpected = false;
                    identifier.Append(character);
                }
                else if (char.IsDigit(character) && identifier.Length > 0)
                {
                    identifierExpected = false;
                    identifier.Append(character);
                }                
                else
                {
                    throw NewUnexpectedCharacterException(templateFormat, characterIndex, character);
                }
                index++;
            }
            if (identifier.Length > 0)
            {
                identifierList.AddLast(identifier);
            }
            if (identifierExpected)
            {
                throw NewMissingIdentifierException(templateFormat, variableStartIndex);
            }
            return CreateVariableComponent(identifierList, format, nextComponent);
        }               

        private static StringTemplateVariable CreateVariableComponent(LinkedList<StringBuilder> identifiers, string format, StringTemplateComponent nextComponent)
        {
            var identifier = identifiers.First.Value.ToString();
            var expression = identifiers.Skip(1).Select(id => id.ToString()).ToArray();

            return new StringTemplateVariable(identifier, expression, format, nextComponent);
        }

        private static Exception NewUnexpectedCharacterException(string templateFormat, int index, char character)
        {
            var messageFormat = ExceptionMessages.StringTemplate_UnexpectedCharacter;
            var message = string.Format(messageFormat, templateFormat, character, index);
            return new ArgumentException(message, "templateFormat");
        }

        private static Exception NewMissingClosingBraceException(string templateFormat, int index)
        {
            var messageFormat = ExceptionMessages.StringTemplate_MissingClosingBrace;
            var message = string.Format(messageFormat, templateFormat, index);
            return new ArgumentException(message, "templateFormat");
        }

        private static Exception NewMissingIdentifierException(string templateFormat, int index)
        {
            var messageFormat = ExceptionMessages.StringTemplate_MissingIdentifier;
            var message = string.Format(messageFormat, templateFormat, index);
            return new ArgumentException(message, "templateFormat");
        }

        #endregion
    }
}

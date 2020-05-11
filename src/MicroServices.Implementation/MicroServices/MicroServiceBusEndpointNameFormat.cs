using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a specific format for the name of an endpoint.
    /// </summary>
    public sealed class MicroServiceBusEndpointNameFormat
    {
        #region [====== Segments ======]

        private static readonly Segment _ServiceNameSegment = new ServiceNameSegment();
        private static readonly Segment _HandlerNameSegment = new HandlerNameSegment();
        private static readonly Segment _MessageNameSegment = new MessageNameSegment();

        private abstract class Segment
        {
            public abstract string Format(string serviceName, string handlerName, string messageName);
        }

        private sealed class FixedSegment : Segment
        {
            private readonly string _value;

            public FixedSegment(string value)
            {
                _value = value;
            }

            public override string Format(string serviceName, string handlerName, string messageName) =>
                _value;

            public override string ToString() =>
                _value;
        }

        private sealed class ServiceNameSegment : Segment
        {
            public const string Name = "service";

            public override string Format(string serviceName, string handlerName, string messageName) =>
                serviceName;

            public override string ToString() =>
                ToPlaceholderName(Name);
        }

        private sealed class HandlerNameSegment : Segment
        {
            public const string Name = "handler";

            public override string Format(string serviceName, string handlerName, string messageName) =>
                handlerName;

            public override string ToString() =>
                ToPlaceholderName(Name);
        }

        private sealed class MessageNameSegment : Segment
        {
            public const string Name = "message";

            public override string Format(string serviceName, string handlerName, string messageName) =>
                messageName;

            public override string ToString() =>
                ToPlaceholderName(Name);
        }

        private static string ToPlaceholderName(string name) =>
            _BracketOpen + name + _BracketClose;

        #endregion

        private readonly Segment[] _segments;

        private MicroServiceBusEndpointNameFormat(IEnumerable<Segment> segments)
        {
            _segments = segments.ToArray();
        }

        #region [====== FormatName ======]

        /// <summary>
        /// Formats the name of an endpoint based on the current format and specified parts.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="messageHandlerType">Type of the message-handler that contains the endpoint.</param>
        /// <param name="messageType">Type of the message received by the endpoint.</param>
        /// <returns>The full name of the endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceName"/>, <paramref name="messageHandlerType"/> or <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public string FormatName(string serviceName, Type messageHandlerType, Type messageType) =>
            FormatName(ResolveServiceName(serviceName), ResolveHandlerName(messageHandlerType), ResolveMessageName(messageType));

        private string FormatName(string serviceName, string handlerName, string messageName) =>
            ToString(segment => segment.Format(serviceName, handlerName, messageName));

        private static string ResolveServiceName(string serviceName) =>
            IsNotNull(serviceName, nameof(serviceName));

        private static string ResolveHandlerName(Type messageHandlerType) =>
            NameOf(IsNotNull(messageHandlerType, nameof(messageHandlerType)));

        private static string ResolveMessageName(Type messageType) =>
            NameOf(IsNotNull(messageType, nameof(messageType)));

        private static string NameOf(Type type) =>
            type.FriendlyName(false, false);

        #endregion

        #region [====== Parse & ToString ======]

        /// <inheritdoc />
        public override string ToString() =>
            ToString(segment => segment.ToString());

        private string ToString(Func<Segment, string> selector) =>
            string.Join(string.Empty, _segments.Select(selector));

        /// <summary>
        /// Parses the specified <paramref name="format"/> and returns a new <see cref="MicroServiceBusEndpointNameFormat"/> that
        /// can be used to resolve the name of a <see cref="IMicroServiceBusEndpoint" />. The format can
        /// contain the placeholders [service], [handler] and [message], which will be used to insert the name of
        /// the service, message handler or message respectively.
        /// </summary>
        /// <param name="format">The format to parse.</param>
        /// <returns>A new <see cref="MicroServiceBusEndpointNameFormat"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is not a valid name-format.
        /// </exception>
        public static MicroServiceBusEndpointNameFormat Parse(string format) =>
            new MicroServiceBusEndpointNameFormat(ParseSegments(format));

        private const char _BracketOpen = '[';
        private const char _BracketClose = ']';

        private static IEnumerable<Segment> ParseSegments(string format)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            var segment = new StringBuilder();
            int? openingBracketIndex = null;
            Segment fixedSegment;

            for (var index = 0; index < format.Length; index++)
            {
                var character = format[index];

                // If we encounter an opening bracket, we assume we'll be parsing
                // a new placeholder. In that case, we consume the bracket, yield
                // the previous segment as fixed segment, and move to the next character.
                // However, if we are already parsing a placeholder the opening bracket
                // is illegal.
                if (character == _BracketOpen)
                {
                    if (openingBracketIndex.HasValue)
                    {
                        throw NewIllegalCharacterException(format, character, index);
                    }
                    if (TryParseFixedSegment(ref segment, out fixedSegment))
                    {
                        yield return fixedSegment;
                    }
                    openingBracketIndex = index;
                    continue;
                }

                // If we encounter a closing bracket, we assume we were parsing
                // a placeholder. In that case, we consume the bracket, yield the
                // placeholder segment and move to the next character.
                // However, if we were not parsing a placeholder, the closing bracket
                // is illegal.
                if (character == _BracketClose)
                {
                    if (openingBracketIndex.HasValue)
                    {
                        openingBracketIndex = null;
                        yield return ParsePlaceholderSegment(ref segment);
                        continue;
                    }
                    throw NewIllegalCharacterException(format, character, index);
                }

                // If we encounter a regular character, we just add it to the current segment.
                segment.Append(character);
            }
            if (openingBracketIndex.HasValue)
            {
                throw NewMissingClosingBracketException(format, openingBracketIndex.Value);
            }
            if (TryParseFixedSegment(ref segment, out fixedSegment))
            {
                yield return fixedSegment;
            }
        }

        private static bool TryParseFixedSegment(ref StringBuilder segment, out Segment fixedSegment)
        {
            var segmentValue = ParseSegmentValue(ref segment);
            if (segmentValue.Length == 0)
            {
                fixedSegment = null;
                return false;
            }
            fixedSegment = new FixedSegment(segmentValue);
            return true;
        }

        private static Segment ParsePlaceholderSegment(ref StringBuilder segment) =>
            ParsePlaceholderSegment(ParseSegmentValue(ref segment));

        private static Segment ParsePlaceholderSegment(string name)
        {
            switch (name)
            {
                case ServiceNameSegment.Name:
                    return _ServiceNameSegment;
                case HandlerNameSegment.Name:
                    return _HandlerNameSegment;
                case MessageNameSegment.Name:
                    return _MessageNameSegment;
                default:
                    throw NewUnknownPlaceholderNameException(name);
            }
        }

        private static string ParseSegmentValue(ref StringBuilder segment) =>
            Interlocked.Exchange(ref segment, new StringBuilder()).ToString();

        private static Exception NewIllegalCharacterException(string format, char character, int index)
        {
            var messageFormat = ExceptionMessages.EndpointNameFormat_IllegalCharacter;
            var message = string.Format(messageFormat, format, character, index);
            return new FormatException(message);
        }

        private static Exception NewUnknownPlaceholderNameException(string name)
        {
            var messageFormat = ExceptionMessages.EndpointNameFormat_UnknownPlaceholderName;
            var message = string.Format(messageFormat, name);
            return new FormatException(message);
        }

        private static Exception NewMissingClosingBracketException(string format, int index)
        {
            var messageFormat = ExceptionMessages.EndpointNameFormat_MissingClosingBracket;
            var message = string.Format(messageFormat, format, index);
            return new FormatException(message);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a name-resolver for endpoints for a specific set of message types.
    /// </summary>
    public sealed class EndpointNameResolver : IEndpointNameResolver
    {
        private readonly Dictionary<Type, EndpointNameFormat> _nameFormatMapping;
        private readonly EndpointNameFormat _defaultNameFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNameResolver" /> class.
        /// </summary>
        /// <param name="defaultNameFormat">
        /// Indicates what the name-format of an endpoint is for messages of which the format
        /// is not explicitly configured.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultNameFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="defaultNameFormat"/> is not valid name-format.
        /// </exception>
        public EndpointNameResolver(string defaultNameFormat)
        {
            _defaultNameFormat = EndpointNameFormat.Parse(defaultNameFormat);
            _nameFormatMapping = new Dictionary<Type, EndpointNameFormat>();
        }

        /// <summary>
        /// Returns the default name-format of an endpoint; this format is applied when
        /// a specific name-format for a certain message is not specified.
        /// </summary>
        public EndpointNameFormat DefaultNameFormat =>
            _defaultNameFormat;

        /// <summary>
        /// Adds a specific name-format for messages of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of a message.</typeparam>
        /// <param name="format">The name-format to use for endpoints handling messages of type <typeparamref name="TMessage"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is not a valid name-format.
        /// </exception>
        public void AddNameFormat<TMessage>(string format) =>
            AddNameFormat(format, typeof(TMessage));

        /// <summary>
        /// Adds a specific name-format for messages which type is specified in <paramref name="messageTypes"/>.
        /// </summary>
        /// <param name="format">
        /// The name-format to use for endpoints handling messages which type is specified in <paramref name="messageTypes"/>.
        /// </param>
        /// <param name="messageTypes">
        /// A collection of message-type to match when resolving the correct name-format to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="messageTypes"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is not a valid name-format.
        /// </exception>
        public void AddNameFormat(string format, params Type[] messageTypes) =>
            AddNameFormat(format, messageTypes.AsEnumerable());

        /// <summary>
        /// Adds a specific name-format for messages which type is specified in <paramref name="messageTypes"/>.
        /// </summary>
        /// <param name="format">
        /// The name-format to use for endpoints handling messages which type is specified in <paramref name="messageTypes"/>.
        /// </param>
        /// <param name="messageTypes">
        /// A collection of message-type to match when resolving the correct name-format to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="format"/> or <paramref name="messageTypes"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is not a valid name-format.
        /// </exception>
        public void AddNameFormat(string format, IEnumerable<Type> messageTypes) =>
            AddNameFormat(EndpointNameFormat.Parse(format), messageTypes.WhereNotNull());

        private void AddNameFormat(EndpointNameFormat format, IEnumerable<Type> messageTypes)
        {
            foreach (var messageType in messageTypes)
            {
                _nameFormatMapping[messageType] = format;
            }
        }

        /// <inheritdoc />
        public string ResolveName(IMicroServiceBusEndpoint endpoint) =>
            FindNameFormatFor(endpoint).ResolveName(endpoint);

        private EndpointNameFormat FindNameFormatFor(IMicroServiceBusEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (_nameFormatMapping.TryGetValue(endpoint.MessageParameterInfo.ParameterType, out var nameFormat))
            {
                return nameFormat;
            }
            return DefaultNameFormat;
        }
    }
}

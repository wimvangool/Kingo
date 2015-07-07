using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server
{
    /// <summary>
    /// Represents a wrapper of a message and it's handler to serve as a <see cref="IQuery{TMessageOut}" />
    /// that can be used within a <see cref="IMessageProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class QueryWrapper<TMessageIn, TMessageOut> : IMessageHandlerOrQuery, IQuery<TMessageOut>
        where TMessageIn : class, IMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly TMessageIn _message;
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly ClassAndMethodAttributeProvider _attributeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWrapper{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="message">Message containing the parameters of the query.</param>
        /// <param name="query">The query to execute.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryWrapper(TMessageIn message, IQuery<TMessageIn, TMessageOut> query)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _message = message;
            _query = query;   
            _attributeProvider = new ClassAndMethodAttributeProvider(query.GetType(), typeof(IQuery<TMessageIn, TMessageOut>));
        }

        /// <inheritdoc />
        public IMessage MessageIn
        {
            get { return _message; }
        }

        /// <inheritdoc />
        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _attributeProvider.TryGetClassAttributeOfType(out attribute);
        }

        /// <inheritdoc />
        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _attributeProvider.TryGetMethodAttributeOfType(out attribute);
        }

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _attributeProvider.GetClassAttributesOfType<TAttribute>();
        }

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _attributeProvider.GetMethodAttributesOfType<TAttribute>();
        }

        /// <inheritdoc />
        public Task<TMessageOut> InvokeAsync()
        {
            return _query.ExecuteAsync(_message);
        }        
    }
}

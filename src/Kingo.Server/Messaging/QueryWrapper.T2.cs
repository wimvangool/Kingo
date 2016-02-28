using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class QueryWrapper<TMessageIn, TMessageOut> : IQueryWrapper<TMessageOut>
        where TMessageIn : class, IMessage
        where TMessageOut : class, IMessage
    {
        private readonly TMessageIn _message;
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly ClassAndMethodAttributeProvider _attributeProvider;
        
        internal QueryWrapper(IQuery<TMessageIn, TMessageOut> query, TMessageIn message)
        {            
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
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
        public Task<TMessageOut> ExecuteAsync()
        {
            return _query.ExecuteAsync(_message);
        }        
    }
}

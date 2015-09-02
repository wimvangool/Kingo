using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    internal sealed class Query<TMessageOut> : IQuery<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IQuery<TMessageOut> _nextQuery;
        private readonly QueryModule _nextModule;

        internal Query(IQuery<TMessageOut> nextQuery, QueryModule nextModule)
        {
            _nextQuery = nextQuery;
            _nextModule = nextModule;
        }

        public IMessage MessageIn
        {
            get { return _nextQuery.MessageIn; }
        }

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _nextQuery.TryGetClassAttributeOfType(out attribute);
        }

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            return _nextQuery.TryGetMethodAttributeOfType(out attribute);
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _nextQuery.GetClassAttributesOfType<TAttribute>();
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return _nextQuery.GetMethodAttributesOfType<TAttribute>();
        }

        public Task<TMessageOut> InvokeAsync()
        {
            return _nextModule.InvokeAsync(_nextQuery);
        }        
    }
}

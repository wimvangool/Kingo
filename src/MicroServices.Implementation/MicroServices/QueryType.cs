using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class QueryType : QueryComponent
    {
        private QueryType(MicroProcessorComponent component, QueryInterface[] interfaces) :
            base(component, interfaces) { }

        #region [====== Factory Methods ======]

        public static QueryType FromInstance<TResponse>(IQuery<TResponse> query)
        {            
            var component = MicroProcessorComponent.FromInstance(query);
            var interfaces = new [] { QueryInterface.FromType<TResponse>() };
            return new QueryType(component, interfaces);
        }
        public static QueryType FromInstance<TRequest, TResponse>(IQuery<TRequest, TResponse> query)
        {
            var component = MicroProcessorComponent.FromInstance(query);
            var interfaces = new [] { QueryInterface.FromType<TRequest, TResponse>() };
            return new QueryType(component, interfaces);
        }

        public static bool IsQuery(MicroProcessorComponent component, out QueryType query)
        {
            var interfaces = QueryInterface.FromComponent(component).ToArray();
            if (interfaces.Length == 0)
            {
                query = null;
                return false;
            }
            query = new QueryType(component, interfaces);
            return true;
        }

        #endregion
    }
}

using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a component that implements one or more variations of the <see cref="IQuery{TResponse}"/>
    /// or <see cref="IQuery{TRequest, TResponse}"/> interfaces.
    /// </summary>
    public sealed class QueryType : Query
    {
        private QueryType(MicroProcessorComponent component, QueryInterface[] interfaces) :
            base(component, interfaces) { }

        #region [====== Factory Methods ======]

        internal new static QueryType FromInstance(object query)
        {            
            var component = MicroProcessorComponent.FromInstance(query);
            var interfaces = QueryInterface.FromComponent(component).ToArray();
            return new QueryType(component, interfaces);
        }

        internal static bool IsQuery(MicroProcessorComponent component, out QueryType query)
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

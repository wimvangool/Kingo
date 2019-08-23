using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class QueryType : Query
    {
        private QueryType(MicroProcessorComponent component, QueryInterface[] interfaces) :
            base(component, interfaces) { }

        #region [====== FromInstance ======]

        public new static QueryType FromInstance(object query)
        {            
            var component = MicroProcessorComponent.FromInstance(query);
            var interfaces = QueryInterface.FromComponent(component).ToArray();
            return new QueryType(component, interfaces);
        }

        #endregion

        #region [====== FromComponent ======]

        public static QueryType FromComponent(MicroProcessorComponent component)
        {
            var interfaces = QueryInterface.FromComponent(component).ToArray();
            if (interfaces.Length == 0)
            {
                return null;
            }
            return new QueryType(component, interfaces);
        }        

        #endregion
    }
}

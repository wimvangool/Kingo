using System;
using System.Collections.Generic;
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

        #region [====== FromComponents ======]

        public static IEnumerable<QueryType> FromComponents(IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var component in components)
            {
                if (IsQueryType(component, out var query))
                {
                    yield return query;
                }
            }
        }

        internal static bool IsQueryType(Type type, out QueryType query)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsQueryType(component, out query);
            }
            query = null;
            return false;
        }

        private static bool IsQueryType(MicroProcessorComponent component, out QueryType query)
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

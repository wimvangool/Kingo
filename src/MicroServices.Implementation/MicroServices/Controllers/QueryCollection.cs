using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a collection of query-types.
    /// </summary>
    public sealed class QueryCollection : MicroProcessorComponentCollection<QueryType>
    {
        #region [====== Add ======]

        protected override bool IsComponentType(MicroProcessorComponent component, out QueryType componentType) =>
            QueryType.IsQuery(component, out componentType);

        #endregion
    }
}

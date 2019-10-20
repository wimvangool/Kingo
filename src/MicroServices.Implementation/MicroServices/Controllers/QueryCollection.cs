using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a collection of query-types.
    /// </summary>
    public sealed class QueryCollection : MicroProcessorComponentCollection
    {
        #region [====== Add ======]

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (QueryType.IsQuery(component, out var query))
            {
                return base.Add(query);
            }
            return false;
        }

        #endregion
    }
}

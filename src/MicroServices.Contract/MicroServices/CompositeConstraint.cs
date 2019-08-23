using System.Collections.Generic;
using System.Linq;
using Kingo.Collections.Generic;

namespace Kingo.MicroServices
{
    internal abstract class CompositeConstraint : Constraint
    {
        protected CompositeConstraint(IEnumerable<IConstraint> constraints)
        {
            Constraints = constraints.WhereNotNull().ToArray();
        }

        protected IConstraint[] Constraints
        {
            get;
        }
    }
}

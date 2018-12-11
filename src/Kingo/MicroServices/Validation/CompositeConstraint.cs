using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices.Validation
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

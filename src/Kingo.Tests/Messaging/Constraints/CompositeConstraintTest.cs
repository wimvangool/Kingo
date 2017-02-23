using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Constraints
{
    [TestClass]
    public abstract class CompositeConstraintTest : ConstraintTestBase
    {
        protected static readonly StringTemplate ParentErrorMessage = StringTemplate.Parse("{parent} is not satisfied.");

        protected static IConstraintWithErrorMessage<object> NewConstraint(bool isSatisfied)
        {
            return new DelegateConstraint<object>(value => isSatisfied);
        }
    }
}

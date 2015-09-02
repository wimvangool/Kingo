using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    [TestClass]
    public sealed class InvalidUsernameScenario : UnitTestScenario<RegisterPlayerCommand>
    {
        protected override RegisterPlayerCommand When()
        {
            var username = PickValueFrom(null, string.Empty, "     ");
            var password = "Password";

            return new RegisterPlayerCommand(username, password);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private static void ContainsExpectedInnerException(IMemberConstraintSet validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException).IsInstanceOf<InvalidUsernameException>();
        }
    }
}

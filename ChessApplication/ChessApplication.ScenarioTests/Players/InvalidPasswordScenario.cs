using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    [TestClass]
    public sealed class InvalidPasswordScenario : UnitTestScenario<RegisterPlayerCommand>
    {
        protected override RegisterPlayerCommand When()
        {
            var username = "Username";
            var password = PickValueFrom(null, string.Empty, "    ", "abc", "asckvbusyfufjshdksdfs");

            return new RegisterPlayerCommand(username, password);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private static void ContainsExpectedInnerException(IMemberConstraintSet validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException).IsInstanceOf<InvalidPasswordException>();
        }
    }
}
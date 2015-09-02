using Kingo.BuildingBlocks.Messaging;
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
            VerifyThatExceptionIsA<InvalidMessageException>().And(validator =>
            {
                validator.VerifyThat(exception => exception.InnerException).IsInstanceOf<InvalidPasswordException>();
            });
        }        
    }
}
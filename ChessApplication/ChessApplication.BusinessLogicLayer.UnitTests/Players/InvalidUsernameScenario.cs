using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Players
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
            VerifyThatExceptionIsA<InvalidMessageException>().And((validator, exception) =>
            
                validator.VerifyThat(() => exception.InnerException).IsInstanceOf<InvalidUsernameException>()
            );
        }
    }
}

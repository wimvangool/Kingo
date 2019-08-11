using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Contracts
{
    [TestClass]
    public sealed class SomeCommandTest : RequestTest<SomeCommand>
    {
        [TestMethod]
        public void SomeCommand_IsNotValid_IfPropertyAIsNull()
        {

        }


    }
}

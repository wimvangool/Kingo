using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public abstract class DataAccessTestStubTest : MicroProcessorTestStubTest<DataAccessTestStub>
    {
        #region [====== CreateMicroProcessorTest ======]

        protected override DataAccessTestStub CreateMicroProcessorTest() =>
            new DataAccessTestStub();

        #endregion
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
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

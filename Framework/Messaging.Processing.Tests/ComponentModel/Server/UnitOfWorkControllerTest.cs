using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Server
{
    [TestClass]
    public sealed class UnitOfWorkControllerTest
    {        
        private UnitOfWorkController _controller;

        [TestInitialize]
        public void Setup()
        {
            _controller = new UnitOfWorkController();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Register_Throws_IfUnitOfWorkIsNull()
        {
            _controller.Enlist(null);
        }        

        
    }
}

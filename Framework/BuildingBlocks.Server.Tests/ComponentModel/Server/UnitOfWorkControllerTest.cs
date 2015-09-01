using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Server
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

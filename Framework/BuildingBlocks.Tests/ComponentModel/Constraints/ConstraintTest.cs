using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    [TestClass]
    public abstract class ConstraintTest
    {        
        [TestInitialize]
        public void Setup()
        {
            RandomErrorMessage = Guid.NewGuid().ToString("N");
        }

        protected string RandomErrorMessage
        {
            get;
            private set;
        }
    }
}

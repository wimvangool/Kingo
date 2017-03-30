using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public abstract class ConstraintTestBase
    {        
        [TestInitialize]
        public virtual void Setup()
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

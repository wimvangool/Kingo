using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    internal sealed class MSTestFramework : IUnitTestFramework
    {
        private MSTestFramework() { }

        public void FailTest()
        {
            Assert.Fail();
        }

        public void FailTest(string message)
        {
            Assert.Fail(message);
        }

        public void FailTest(string message, params object[] parameters)
        {
            Assert.Fail(message, parameters);
        }

        public static readonly MSTestFramework Instance = new MSTestFramework();
    }
}

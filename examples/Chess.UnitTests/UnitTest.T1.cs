using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess
{
    [TestClass]
    public abstract class UnitTest<TMessage> : UnitTestBase<TMessage>
    {
        protected override IMicroProcessor CreateProcessor() =>
            new UnitTestProcessor();

        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new AssertFailedException(message, innerException);
    }
}

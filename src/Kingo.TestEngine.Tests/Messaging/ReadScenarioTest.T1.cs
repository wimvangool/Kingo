using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public abstract class ReadScenarioTest<TMessageOut> : ReadScenario<TMessageOut>
    {
        protected override IMicroProcessor CreateProcessor() =>
            new MicroProcessor();

        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new MetaAssertFailedException(message, innerException);
    }
}

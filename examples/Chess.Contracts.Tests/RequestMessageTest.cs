using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess
{
    public abstract class RequestMessageTest : RequestMessageTestBase
    {
        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new AssertFailedException(message, innerException);
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Constraints
{
    [TestClass]
    public sealed partial class StringConstraintsTest : ConstraintTestBase
    {                                                                                        
        private static ValidatedMessage<string> NewValidatedMessage()
        {
            return new ValidatedMessage<string>(Guid.NewGuid().ToString("N"));
        }
    }
}

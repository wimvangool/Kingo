using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed partial class StringConstraintsTest : ConstraintTestBase
    {                                                                                        
        private static ValidatedMessage<string> NewValidatedMessage() => new ValidatedMessage<string>(Guid.NewGuid().ToString("N"));
    }
}

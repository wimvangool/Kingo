using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class VerifyingOutputState : MicroProcessorTestState
    {
        protected void MoveToEndState() =>
            Test.MoveToState(this, new NotReadyToConfigureState(Test));

        protected static Exception NewOutputAssertionFailedException(Exception exception) =>
            new TestFailedException(exception.Message, exception);
    }
}

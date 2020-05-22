using System;
using Kingo.MicroServices.DataContracts;

namespace Kingo.MicroServices
{
    internal abstract class VerifyingOutputState : MicroProcessorTestState
    {
        protected void VerifyThat(ITestOutputAssertMethod method)
        {
            try
            {
                method.Execute();
            }
            catch (TestFailedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewOutputAssertionFailedException(exception);
            }
            finally
            {
                MoveToEndState();
            }
        }

		private void MoveToEndState() =>
            Test.MoveToState(this, new NotReadyToConfigureState(Test));

        private static Exception NewOutputAssertionFailedException(Exception exception) =>
            new TestFailedException(exception.Message, exception);
    }
}

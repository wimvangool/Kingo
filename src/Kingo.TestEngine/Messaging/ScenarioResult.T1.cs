using System;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal abstract class ScenarioResult<TResult> : IScenarioResult
    {
        public async Task IsExceptionOfTypeAsync<TException>(Action<TException> assertCallback = null) where TException : ExternalProcessorException
        {
            await SetupScenarioAsync();            

            try
            {
                await ExecuteScenarioAsync();
            }
            catch (TException exception)
            {
                assertCallback?.Invoke(exception);
                return;
            }
            catch (Exception exception)
            {
                throw NewExceptionAssertFailedException(typeof(TException), exception);
            }
            finally
            {
                await TearDownScenarioAsync();
            }
            throw NewExceptionAssertFailedException(typeof(TException));
        }

        protected abstract Task SetupScenarioAsync();

        protected abstract Task<TResult> ExecuteScenarioAsync();

        protected abstract Task TearDownScenarioAsync();

        private Exception NewExceptionAssertFailedException(Type expectedExceptionType, Exception actualException)
        {
            var messageFormat = ExceptionMessages.Scenario_ExceptionAssertFailed_OtherExceptionType;
            var message = string.Format(messageFormat, expectedExceptionType.FriendlyName(), actualException.GetType().FriendlyName());
            return NewAssertFailedException(message, actualException);
        }

        private Exception NewExceptionAssertFailedException(Type expectedExceptionType)
        {
            var messageFormat = ExceptionMessages.Scenario_ExceptionAssertFailed_NoException;
            var message = string.Format(messageFormat, expectedExceptionType.FriendlyName());
            return NewAssertFailedException(message, null);
        }

        protected abstract Exception NewAssertFailedException(string message, Exception exception);
    }
}

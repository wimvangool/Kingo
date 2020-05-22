using System;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MicroProcessorTestState
    {
        protected abstract MicroProcessorTest Test
        {
            get;
        }

        #region [====== Setup & TearDown ======]

        public virtual Task SetupAsync() =>
            throw NewSetupFailedException(nameof(SetupAsync));

        public virtual Task TearDownAsync() =>
            throw NewTearDownFailedException(nameof(TearDownAsync));

        #endregion

        #region [====== Given & When ======]

        public virtual IGivenState Given() =>
            throw NewInvalidOperationException(nameof(Given));

        public virtual IWhenBusinessLogicTestState WhenBusinessLogicTest() =>
            throw NewInvalidOperationException(nameof(WhenBusinessLogicTest));

        public virtual IWhenDataAccessTestState WhenDataAccessTest() =>
            throw NewInvalidOperationException(nameof(WhenDataAccessTest));

        #endregion

        #region [====== Exception Factory Methods ======]

        private Exception NewSetupFailedException(string methodName)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_SetupFailed;
            var message = string.Format(messageFormat, Test.GetType().FriendlyName(), methodName, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        private Exception NewTearDownFailedException(string methodName)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_TearDownFailed;
            var message = string.Format(messageFormat, Test.GetType().FriendlyName(), methodName, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        protected Exception NewInvalidOperationException(string methodName)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_InvalidOperation;
            var message = string.Format(messageFormat, Test.GetType().FriendlyName(), methodName, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion
    }
}

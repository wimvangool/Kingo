using System;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
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

        public virtual IWhenCommandState<TCommand> WhenCommand<TCommand>() =>
            throw NewInvalidOperationException(nameof(WhenCommand));

        public virtual IWhenEventState<TEvent> WhenEvent<TEvent>() =>
            throw NewInvalidOperationException(nameof(WhenEvent));

        public virtual IWhenRequestState WhenRequest() =>
            throw NewInvalidOperationException(nameof(WhenRequest));

        public virtual IWhenRequestState<TRequest> WhenRequest<TRequest>() =>
            throw NewInvalidOperationException(nameof(WhenRequest));

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

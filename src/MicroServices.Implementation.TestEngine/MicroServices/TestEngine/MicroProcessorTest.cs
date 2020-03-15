using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.Configuration;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Serves as a base-class for all tests that verify the behavior of message handlers or queries.
    /// </summary>    
    public abstract class MicroProcessorTest
    {
        private readonly Lazy<IServiceProvider> _serviceProvider;
        private readonly ClaimsPrincipal _user;
        private MicroProcessorTestState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorTest" /> class.
        /// </summary>
        internal MicroProcessorTest()
        {
            _serviceProvider = new Lazy<IServiceProvider>(CreateServiceProvider);
            _user = new ClaimsPrincipal(new GenericIdentity($"{Environment.UserDomainName}\\{Environment.UserName}", "Anonymous"));
            _state = new NotReadyToConfigureState(this);
        }

        internal MicroProcessorTestContext CreateTestContext() =>
            new MicroProcessorTestContext(this, ServiceProvider.GetRequiredService<IMicroProcessor>());

        /// <summary>
        /// Returns the default <see cref="ClaimsPrincipal" /> that is used to run operations for
        /// which no explicit user has been configured.
        /// </summary>
        protected internal virtual ClaimsPrincipal User =>
            _user;

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()}: {_state}";

        #region [====== ServiceProvider ======]

        /// <summary>
        /// Returns the <see cref="IServiceProvider" /> used by the <see cref="IMicroProcessor" />.
        /// </summary>
        protected IServiceProvider ServiceProvider =>
            _serviceProvider.Value;

        private IServiceProvider CreateServiceProvider() =>
            ConfigureServices(new ServiceCollection()).BuildServiceProvider(true);

        /// <summary>
        /// Configures <paramref name="services"/> with a <see cref="IMicroProcessor" /> and other
        /// dependencies for the tests to run.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        protected virtual IServiceCollection ConfigureServices(IServiceCollection services) =>
            services.AddMicroProcessor();

        #endregion

        #region [====== State ======]

        internal MicroProcessorTestState State =>
            _state;

        internal TState MoveToState<TState>(MicroProcessorTestState expectedState, TState desiredState) where TState : MicroProcessorTestState
        {
            if (Interlocked.CompareExchange(ref _state, desiredState, expectedState) == expectedState)
            {
                return desiredState;
            }
            throw NewCannotMoveToStateException(expectedState, desiredState);
        }

        private static Exception NewCannotMoveToStateException(MicroProcessorTestState expectedState, MicroProcessorTestState desiredState)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_CannotMoveToState;
            var message = string.Format(messageFormat, desiredState.GetType().FriendlyName(), expectedState.GetType().FriendlyName());
            return new InvalidOperationException(message);
        }

        #endregion

        #region [====== Setup & TearDown ======]

        /// <summary>
        /// Prepares the next test for execution.
        /// </summary>
        public virtual Task SetupAsync() =>
            _state.SetupAsync();

        /// <summary>
        /// Resets the test-class and cleans up any resources used by the last test.
        /// </summary>
        public virtual Task TearDownAsync() =>
            _state.TearDownAsync();

        #endregion

        #region [====== Given ======]

        /// <summary>
        /// Prepares the specified <typeparamref name="TMessage"/> to be handled or executed by a
        /// <see cref="IMessageHandler{TMessage}" /> during the setup-phase of a test.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <returns>
        /// A <see cref="IGivenCommandOrEventState{TMessage}"/> that can be used to configure which
        /// <see cref="IMessageHandler{TMessage}"/> to use when handling or executing the specified message.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IGivenCommandOrEventState<TMessage> Given<TMessage>() =>
            Given().Message<TMessage>();

        /// <summary>
        /// Returns the <see cref="IGivenState" /> that can be used to setup a test.
        /// </summary>
        /// <returns>The <see cref="IGivenState"/> of the current test.</returns>
        /// <exception cref="InvalidOperationException">
        /// The test-engine is not in a state where it can perform this operation.
        /// </exception>
        protected IGivenState Given() =>
            _state.Given();

        #endregion
    }
}

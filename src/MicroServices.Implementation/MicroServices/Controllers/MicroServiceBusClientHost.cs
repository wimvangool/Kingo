using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Controllers
{
    internal abstract class MicroServiceBusClientHost : MicroServiceBusClient
    {
        #region [====== State ======]

        private abstract class State : AsyncDisposable
        {
            public abstract MicroServiceBusClient Client
            {
                get;
            }

            public override string ToString() =>
                GetType().FriendlyName().RemovePostfix(nameof(State));

            public abstract Task StartAsync(CancellationToken token);

            public abstract Task StopAsync();

            protected override async ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    await Client.DisposeAsync();
                }
                await base.DisposeAsync(context);
            }
        }

        #endregion

        #region [====== StoppedState ======]

        private sealed class StoppedState : State
        {
            private readonly MicroServiceBusClientHost _host;
            private readonly StoppedClient _client;

            public StoppedState(MicroServiceBusClientHost host)
            {
                _host = host;
                _client = new StoppedClient(host);
            }

            public override MicroServiceBusClient Client =>
                _client;

            public override Task StartAsync(CancellationToken token) =>
                StartAsync(new StartingState(_host, token));

            private async Task StartAsync(StartingState startingState)
            {
                // When starting the client, we first move to the starting state. While in this state,
                // the client is created, initialized and returned. When the client is returned, the
                // client-host is moved to the started state. If this process, for whatever reason,
                // fails, then the client-host is reverted back to a stopped state.
                if (await _host.MoveToStateAsync(this, startingState).ConfigureAwait(false))
                {
                    try
                    {
                        // While the client is being created, another thread may have called StopAsync() on the host.
                        // If that's the case, the specified token will be signaled, so that the startup-operation knows it
                        // can cancel its operation.
                        // In addition, we can no longer move into the started-state (since the current state is no longer the
                        // starting state). This means this state can then immediately be disposed.
                        var startedState = await CreateStartedStateAsync(startingState.Token).ConfigureAwait(false);

                        if (await _host.MoveToStateAsync(startingState, startedState).ConfigureAwait(false))
                        {
                            return;
                        }
                        await startedState.DisposeAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        await _host.MoveToStateAsync(startingState, new StoppedState(_host)).ConfigureAwait(false);
                        throw;
                    }
                }
                else
                {
                    throw NewClientAlreadyRunningException(_host);
                }
            }

            private async Task<StartedState> CreateStartedStateAsync(CancellationToken token) =>
                new StartedState(_host, await _host.CreateClientAsync(token).ConfigureAwait(false));

            public override Task StopAsync() =>
                Task.CompletedTask;
        }

        #endregion

        #region [====== StartingState ======]

        private sealed class StartingState : State
        {
            private readonly MicroServiceBusClientHost _host;
            private readonly StoppedClient _client;
            private readonly CancellationTokenSource _tokenSource;

            public StartingState(MicroServiceBusClientHost host, CancellationToken token)
            {
                _host = host;
                _client = new StoppedClient(host);
                _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            }

            public override MicroServiceBusClient Client =>
                _client;

            public CancellationToken Token =>
                _tokenSource.Token;

            public override Task StartAsync(CancellationToken token) =>
                throw NewClientAlreadyRunningException(_host);

            public override async Task StopAsync()
            {
                // If StopAsync is called when the host is still starting, we signal
                // to the startup-operation that it should cancel its efforts.
                if (await _host.MoveToStateAsync(this, new StoppedState(_host)).ConfigureAwait(false))
                {
                    _tokenSource.Cancel();
                }
            }

            protected override ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    _tokenSource.Dispose();
                }
                return base.DisposeAsync(context);
            }
        }

        #endregion

        #region [====== StartedState ======]

        private sealed class StartedState : State
        {
            private readonly MicroServiceBusClientHost _host;
            private readonly MicroServiceBusClient _client;

            public StartedState(MicroServiceBusClientHost host, MicroServiceBusClient client)
            {
                _host = host;
                _client = client;
            }

            public override MicroServiceBusClient Client =>
                _client;

            public override Task StartAsync(CancellationToken token) =>
                throw NewClientAlreadyRunningException(_host);

            public override Task StopAsync() =>
                _host.MoveToStateAsync(this, new StoppedState(_host));

            protected override async ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    await _client.DisposeAsync();
                }
                await base.DisposeAsync(context);
            }
        }

        #endregion

        #region [====== DisposedState ======]

        private sealed class DisposedState : State
        {
            private readonly MicroServiceBusClientHost _host;
            private readonly DisposedClient _client;

            public DisposedState(MicroServiceBusClientHost host)
            {
                _host = host;
                _client = new DisposedClient(_host);
            }

            public override MicroServiceBusClient Client =>
                _client;

            public override Task StartAsync(CancellationToken token) =>
                throw NewClientDisposedException(_host);

            public override Task StopAsync() =>
                throw NewClientDisposedException(_host);
        }

        #endregion

        #region [====== StoppedClient ======]

        private sealed class StoppedClient : MicroServiceBusClient
        {
            private readonly MicroServiceBusClientHost _host;

            public StoppedClient(MicroServiceBusClientHost host)
            {
                _host = host;
            }

            protected override Task SendAsync(IMessage[] messages) =>
                throw NewClientNotRunningException(_host);

            protected override TransactionScope CreateTransactionScope() =>
                new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        }

        #endregion

        #region [====== DisposedClient ======]

        private sealed class DisposedClient : MicroServiceBusClient
        {
            private readonly MicroServiceBusClientHost _host;

            public DisposedClient(MicroServiceBusClientHost host)
            {
                _host = host;
            }

            protected override Task SendAsync(IMessage[] messages) =>
                throw NewClientDisposedException(_host);

            protected override TransactionScope CreateTransactionScope() =>
                new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        }

        #endregion

        private State _state;

        protected MicroServiceBusClientHost()
        {
            _state = new StoppedState(this);
        }

        protected abstract string Name
        {
            get;
        }

        protected abstract MessageDirection SupportedMessageDirection
        {
            get;
        }

        public override string ToString() =>
            _state.ToString();

        protected override Task SendAsync(IMessage[] messages) =>
            _state.Client.SendAsync(IsNotNull(messages, nameof(messages)).Where(IsSupportedMessage));

        private bool IsSupportedMessage(IMessage message) =>
            message != null && message.Direction == SupportedMessageDirection;

        #region [====== StartAsync(...), StopAsync(...) & DisposeAsync(...) ======]

        public Task StartAsync(CancellationToken token) =>
            _state.StartAsync(token);

        protected abstract Task<MicroServiceBusClient> CreateClientAsync(CancellationToken token);

        public Task StopAsync() =>
            _state.StopAsync();

        protected override async ValueTask DisposeAsync(DisposeContext context)
        {
            if (context != DisposeContext.Finalizer)
            {
                await Interlocked.Exchange(ref _state, new DisposedState(this)).DisposeAsync().ConfigureAwait(false);
            }
            await base.DisposeAsync(context);
        }

        private static Exception NewClientNotRunningException(MicroServiceBusClientHost host)
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ClientNotRunning;
            var message = string.Format(messageFormat, host.Name);
            return new InvalidOperationException(message);
        }

        private static Exception NewClientAlreadyRunningException(MicroServiceBusClientHost host)
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ClientAlreadyRunning;
            var message = string.Format(messageFormat, host.Name);
            return new InvalidOperationException(message);
        }

        private static ObjectDisposedException NewClientDisposedException(MicroServiceBusClientHost host) =>
            host.NewObjectDisposedException();

        #endregion

        #region [====== MoveToState ======]

        private async Task<bool> MoveToStateAsync(State oldState, State newState)
        {
            try
            {
                return MoveToState(oldState, newState);
            }
            finally
            {
                await oldState.DisposeAsync().ConfigureAwait(false);
            }
        }

        private bool MoveToState(State oldState, State newState) =>
            Interlocked.CompareExchange(ref _state, newState, oldState) == oldState;

        #endregion
    }
}

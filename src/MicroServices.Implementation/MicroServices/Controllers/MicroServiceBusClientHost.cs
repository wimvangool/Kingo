using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

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

            public override ValueTask DisposeAsync() =>
                Client.DisposeAsync();
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
                // fails, then the client-host is reverted back to a stopped state. if
                if (await _host.MoveToStateAsync(this, startingState).ConfigureAwait(false))
                {
                    try
                    {
                        // While the client is being created, another thread may have called StopAsync() on the host.
                        // For this reason, we check if the host is successfully moved into the started state. If not,
                        // the client that was just created remains unused and can be disposed immediately.
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
                throw _host.NewClientAlreadyRunningException();
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
                throw _host.NewClientAlreadyRunningException();

            public override Task StopAsync() =>
                _host.MoveToStateAsync(this, new StoppedState(_host));

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // The starting-state might be disposed because another thread just opted to
                    // stop the client, in which case we want to cancel the startup-operation.
                    _tokenSource.Cancel();
                    _tokenSource.Dispose();
                }
                base.Dispose(disposing);
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
                throw _host.NewClientAlreadyRunningException();

            public override Task StopAsync() =>
                _host.MoveToStateAsync(this, new StoppedState(_host));

            public override ValueTask DisposeAsync() =>
                _client.DisposeAsync();
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

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                throw _host.NewClientNotRunningException();
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

        public override string ToString() =>
            _state.ToString();

        public override Task SendAsync(IEnumerable<IMessage> messages) =>
            _state.Client.SendAsync(messages);

        #region [====== StartAsync(...), StopAsync(...) & DisposeAsync(...) ======]

        public Task StartAsync(CancellationToken token) =>
            _state.StartAsync(token);

        protected abstract Task<MicroServiceBusClient> CreateClientAsync(CancellationToken token);

        public Task StopAsync() =>
            _state.StopAsync();

        public override ValueTask DisposeAsync() =>
            _state.DisposeAsync();

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

        #region [======= Exception Factory Methods ======]

        private Exception NewClientNotRunningException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ClientNotRunning;
            var message = string.Format(messageFormat, Name);
            return new InvalidOperationException(message);
        }

        private Exception NewClientAlreadyRunningException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ClientAlreadyRunning;
            var message = string.Format(messageFormat, Name);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}

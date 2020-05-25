using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusProxyManager : AsyncDisposable
    {
        #region [====== State ======]

        private abstract class State : AsyncDisposable
        {
            public abstract MicroServiceBusProxy Proxy
            {
                get;
            }

            public override string ToString() =>
                GetType().FriendlyName().RemovePostfix(nameof(State));

            public abstract Task StartAsync(CancellationToken token);

            public abstract Task StopAsync();

            public override ValueTask DisposeAsync() =>
                Proxy.DisposeAsync();
        }

        #endregion

        #region [====== StoppedState ======]

        private sealed class StoppedState : State
        {
            private readonly MicroServiceBusProxyManager _manager;
            private readonly StoppedProxy _proxy;

            public StoppedState(MicroServiceBusProxyManager manager)
            {
                _manager = manager;
                _proxy = new StoppedProxy(manager);
            }

            public override MicroServiceBusProxy Proxy =>
                _proxy;

            public override Task StartAsync(CancellationToken token) =>
                StartAsync(token, new StartingState(_manager));

            private async Task StartAsync(CancellationToken token, StartingState startingState)
            {
                // When starting the proxy, we first move to the starting state. While in this state,
                // the proxy is created, initialized and returned. When the proxy is returned, the
                // proxy-manager is moved to the started state. If this process, for whatever reason,
                // fails, then the proxy-manager is reverted back to a stopped state. if
                if (await _manager.MoveToStateAsync(this, startingState).ConfigureAwait(false))
                {
                    try
                    {
                        // While the proxy is being created, another thread may have called StopAsync() on the manager.
                        // For this reason, we check if the manager is successfully moved into the started state. If not,
                        // the proxy that was just created remains unused and can be disposed immediately.
                        var startedState = await StartProxyAsync(token).ConfigureAwait(false);

                        if (await _manager.MoveToStateAsync(startingState, startedState).ConfigureAwait(false))
                        {
                            return;
                        }
                        await startedState.DisposeAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        await _manager.MoveToStateAsync(startingState, new StoppedState(_manager)).ConfigureAwait(false);
                        throw;
                    }
                }
                throw _manager.NewProxyAlreadyRunningException();
            }

            private async Task<State> StartProxyAsync(CancellationToken token) =>
                new StartedState(_manager, await _manager._proxyFactory.Invoke(token));

            public override Task StopAsync() =>
                Task.CompletedTask;
        }

        #endregion

        #region [====== StartingState ======]

        private sealed class StartingState : State
        {
            private readonly MicroServiceBusProxyManager _manager;
            private readonly StoppedProxy _proxy;

            public StartingState(MicroServiceBusProxyManager manager)
            {
                _manager = manager;
                _proxy = new StoppedProxy(manager);
            }

            public override MicroServiceBusProxy Proxy =>
                _proxy;

            public override Task StartAsync(CancellationToken token) =>
                throw _manager.NewProxyAlreadyRunningException();

            public override Task StopAsync() =>
                _manager.MoveToStateAsync(this, new StoppedState(_manager));
        }

        #endregion

        #region [====== StartedState ======]

        private sealed class StartedState : State
        {
            private readonly MicroServiceBusProxyManager _manager;
            private readonly MicroServiceBusProxy _proxy;

            public StartedState(MicroServiceBusProxyManager manager, MicroServiceBusProxy proxy)
            {
                _manager = manager;
                _proxy = proxy;
            }

            public override MicroServiceBusProxy Proxy =>
                _proxy;

            public override Task StartAsync(CancellationToken token) =>
                throw _manager.NewProxyAlreadyRunningException();

            public override Task StopAsync() =>
                _manager.MoveToStateAsync(this, new StoppedState(_manager));
        }

        #endregion

        #region [====== StoppedProxy ======]

        private sealed class StoppedProxy : MicroServiceBusProxy
        {
            private readonly MicroServiceBusProxyManager _manager;

            public StoppedProxy(MicroServiceBusProxyManager manager)
            {
                _manager = manager;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                throw _manager.NewProxyNotRunningException();
        }

        #endregion

        private readonly string _proxyName;
        private readonly Func<CancellationToken, Task<MicroServiceBusProxy>> _proxyFactory;
        private State _state;

        public MicroServiceBusProxyManager(string proxyName, Func<CancellationToken, Task<MicroServiceBusProxy>> proxyFactory)
        {
            _proxyName = proxyName.ToLowerInvariant();
            _proxyFactory = proxyFactory;
            _state = new StoppedState(this);
        }

        public IMicroServiceBus Proxy =>
            _state.Proxy;

        public override string ToString() =>
            _state.ToString();

        public Task StartAsync(CancellationToken token) =>
            _state.StartAsync(token);

        public Task StopAsync() =>
            _state.StopAsync();

        public override ValueTask DisposeAsync() =>
            _state.DisposeAsync();

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

        private Exception NewProxyNotRunningException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ProxyNotRunning;
            var message = string.Format(messageFormat, _proxyName);
            return new InvalidOperationException(message);
        }

        private Exception NewProxyAlreadyRunningException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_ProxyAlreadyRunning;
            var message = string.Format(messageFormat, _proxyName);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}

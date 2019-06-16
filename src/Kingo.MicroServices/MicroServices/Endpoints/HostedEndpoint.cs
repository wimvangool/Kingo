using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented, represents an endpoint that will connect to its resource upon start and
    /// disconnect upon stop.
    /// </summary>
    public abstract class HostedEndpoint : Disposable, IHostedService
    {        
        private bool _isConnected;

        /// <inheritdoc />
        public override string ToString() =>
            ToString(GetType());

        internal string ToString(Type endpointType) =>
            $"{endpointType.FriendlyName()} (Connected = {_isConnected})";

        /// <inheritdoc />
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isConnected)
            {
                throw NewEndpointAlreadyStartedException();
            }
            await ConnectAsync(cancellationToken);

            _isConnected = true;
        }

        /// <inheritdoc />
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isConnected)
            {
                try
                {
                    await Task.WhenAny(DisconnectAsync(cancellationToken), Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken));
                }
                finally
                {
                    _isConnected = false;
                }
            }            
        }                         

        /// <summary>
        /// Connects this endpoint to its resource. If <paramref name="cancellationToken"/> is signaled while
        /// the operation is still in progress, the operation is expected to be aborted and the endpoint
        /// must remain in its disconnected state.
        /// </summary>
        /// <param name="cancellationToken">Token used to signal cancellation of this operation.</param>        
        protected abstract Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Disconnects this endpoint from its resource. If <paramref name="cancellationToken"/> is signaled while
        /// the operation is still in progress, the operation is expected to be aborted immediately, and the state
        /// of the endpoint may be undetermined.
        /// </summary>
        /// <param name="cancellationToken">Token used to signal cancellation of this operation.</param>        
        protected abstract Task DisconnectAsync(CancellationToken cancellationToken);

        private Exception NewEndpointAlreadyStartedException()
        {
            var messageFormat = ExceptionMessages.HostedEndpoint_EndpointAlreadyStarted;
            var message = string.Format(messageFormat, GetType().FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}

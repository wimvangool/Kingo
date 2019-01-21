using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorTestContext : IMicroServiceBus
    {        
        private readonly Dictionary<IMicroProcessorTest, EventStream> _eventStreams;
        private MemoryServiceBus _serviceBus;

        private MicroProcessorTestContext()
        {                        
            _eventStreams = new Dictionary<IMicroProcessorTest, EventStream>();
            _serviceBus = new MemoryServiceBus();
        }

        #region [====== IMicroServiceBus ======]

        Task IMicroServiceBus.PublishAsync(IEnumerable<object> messages) =>
            _serviceBus.PublishAsync(messages);

        Task IMicroServiceBus.PublishAsync(object message) =>
            _serviceBus.PublishAsync(message);

        #endregion

        #region [====== SetEventStream ======]

        internal void SetEventStream<TEventStream>(IMicroProcessorTest test, TEventStream stream) where TEventStream : EventStream
        {
            try
            {
                _eventStreams.Add(test, stream);
            }
            catch (ArgumentException exception)
            {
                throw NewTestAlreadyRunException(test, exception);
            }
        }

        internal EventStream CommitEventStream() =>
            new EventStream(Interlocked.Exchange(ref _serviceBus, new MemoryServiceBus()));

        private static Exception NewTestAlreadyRunException(object test, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestAlreadyRun;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new InvalidOperationException(message, innerException);
        }

        #endregion

        #region [====== GetEventStream ======]        

        /// <summary>
        /// Returns the <see cref="EventStream"/> that was produced by the specified <paramref name="test"/> and stored in this context.
        /// </summary>        
        /// <param name="test">The test that produced the event-stream.</param>
        /// <returns>The event-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No event-stream produced by the specified <paramref name="test"/> was stored in this context.
        /// </exception>
        public TEventStream GetEventStream<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            try
            {
                return (TEventStream) _eventStreams[test];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewEventStreamNotFoundException(test, exception);
            }            
        }

        private static Exception NewEventStreamNotFoundException(object test, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_EventStreamNotFound;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new ArgumentException(message, nameof(test), innerException);
        }

        #endregion

        #region [====== ServiceBus ======]

        private sealed class ServiceBusRelay : MicroServiceBus
        {
            public override Task PublishAsync(object message) =>
                CurrentBus.PublishAsync(message);
        }

        internal static readonly IMicroServiceBus ServiceBus = new ServiceBusRelay();

        private static IMicroServiceBus CurrentBus
        {
            get
            {
                var bus = Current;
                if (bus == null)
                {
                    return MicroServiceBus.Null;
                }
                return bus;
            }
        }

        #endregion

        #region [====== Current ======]

        private static readonly Context<MicroProcessorTestContext> _Context = new Context<MicroProcessorTestContext>();        

        /// <summary>
        /// Returns the current <see cref="MicroProcessorTestContext"/>, or <c>null</c> if no test is currently running.
        /// </summary>
        public static MicroProcessorTestContext Current =>
            _Context.Current;

        internal static IDisposable CreateScope(IServiceProvider serviceProvider) =>
            new DisposableComposite(_Context.OverrideAsyncLocal(new MicroProcessorTestContext()), serviceProvider.CreateScope());

        #endregion
    }
}

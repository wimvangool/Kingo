using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorTestContext
    {
        #region [====== GivenAsync ======]

        /// <summary>
        /// Handles the specified <paramref name="message"/>, optionally using the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler that is used to handle the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.        
        /// </exception> 
        public Task GivenAsync<TMessage>(TMessage message, Func<TMessage, MessageHandlerContext, Task> handler) =>
            GivenAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler));

        /// <summary>
        /// Handles the specified <paramref name="message"/>, optionally using the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler that is used to handle the message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.        
        /// </exception> 
        public Task GivenAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs the specified <paramref name="test"/> and stores it's resulting <typeparamref name="TEventStream"/>,
        /// so that it can be retrieved later by other test's using <paramref name="test"/> as the retrieval key.
        /// </summary>        
        /// <param name="test">The test to run.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        public Task GivenAsync<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== GetEventStream ======]

        /// <summary>
        /// Returns the <see cref="EventStream"/> that was produced by handling the specified <paramref name="message" /> and stored in this context.
        /// </summary>        
        /// <param name="message">The message that was handled to produce the event-stream.</param>
        /// <returns>The event-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No event-stream produced by handling the specified <paramref name="message"/> was stored in this context.
        /// </exception>
        public EventStream GetEventStream<TMessage>(TMessage message) =>
            throw new NotImplementedException();

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
        public TEventStream GetEventStream<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

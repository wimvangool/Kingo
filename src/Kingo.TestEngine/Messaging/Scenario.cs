using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>    
    public abstract class Scenario : IMessageStream
    {
        private readonly Lazy<IMicroProcessor> _processor;
        private readonly Lazy<IMessageStream> _stream;

        internal Scenario()
        {
            _processor = new Lazy<IMicroProcessor>(CreateProcessor);
            _stream = new Lazy<IMessageStream>(CreateStream);
        }

        /// <summary>
        /// Returns the processor that is used to execute this <see cref="Scenario{T}"/>.
        /// </summary>
        protected IMicroProcessor Processor =>
            _processor.Value;

        /// <summary>
        /// Creates and returns the processor that is used to execute this <see cref="Scenario{T}" />.
        /// </summary>
        protected abstract IMicroProcessor CreateProcessor();

        internal virtual IMessageStream CreateStream() =>
            Given().Join();

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()}: {Stream}";

        #region [====== IMessageStream ======]

        private IMessageStream Stream =>
            _stream.Value;

        int IReadOnlyCollection<object>.Count =>
            Stream.Count;

        object IReadOnlyList<object>.this[int index] =>
            Stream[index];

        IEnumerator<object> IEnumerable<object>.GetEnumerator() =>
            Stream.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Stream.GetEnumerator();

        IMessageStream IMessageStream.Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler) =>
            Stream.Append(message, handler);

        IMessageStream IMessageStream.AppendStream(IMessageStream stream) =>
            Stream.AppendStream(stream);

        Task IMessageStream.HandleMessagesWithAsync(IMessageHandler handler) =>
            Stream.HandleMessagesWithAsync(handler);

        #endregion

        #region [====== Setup & TearDown ======]

        /// <summary>
        /// Performs all necessary initialization of this scenario before it is executed.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        protected virtual async Task SetupAsync() =>
            OnStateInitialized(await Processor.HandleStreamAsync(Given().Join()));

        /// <summary>
        /// This method is invoked once the scenario has initialized its begin state by processing
        /// all the message streams returns by the <see cref="Given()"/> method during the setup phase.
        /// </summary>
        /// <param name="stream">The stream of events that were publised during the setup phase.</param>
        protected virtual void OnStateInitialized(IMessageStream stream) =>
            Console.WriteLine($"{GetType().FriendlyName()}: state initialized. Event Count: {stream.Count}");

        /// <summary>
        /// Performs all necessary cleanup after this scenario has been executed.
        /// </summary>
        /// <returns>A task representing the operation.</returns>
        protected virtual Task TearDownAsync() =>
            AsyncMethod.Void;

        #endregion

        #region [====== Given / When / Then ======]        

        /// <summary>
        /// Returns a collection of streams that are processed by the <see cref="Processor" /> to setup a specific domain state.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<IMessageStream> Given() =>
            Enumerable.Empty<IMessageStream>();

        /// <inheritdoc />
        public abstract Task ThenAsync();

        #endregion

        #region [====== Assertion ======]

        /// <summary>
        /// Asserts that the inner exception of another exception is an instance of a specific type and invokes a callback
        /// which can be used to assert the properties of the inner exception.
        /// </summary>
        /// <typeparam name="TException">Type of the expected (inner) exception.</typeparam>
        /// <param name="exception">An exception.</param>
        /// <param name="assertCallback">
        /// Callback that can be used to assert the properties of the inner exception.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception"/> or <paramref name="assertCallback" /> is <c>null</c>.
        /// </exception>
        protected void AssertInnerException<TException>(Exception exception, Action<TException> assertCallback) where TException : Exception
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (assertCallback == null)
            {
                throw new ArgumentNullException(nameof(assertCallback));
            }
            assertCallback.Invoke(GetInnerException<TException>(exception));
        }

        private TException GetInnerException<TException>(Exception exception) where TException : Exception
        {
            if (exception.InnerException == null)
            {
                throw NewInnerExceptionNotFoundException(typeof(TException), exception.GetType());
            }
            try
            {
                return (TException) exception.InnerException;
            }
            catch (InvalidCastException)
            {
                throw NewInnerExceptionOfDifferentTypeException(typeof(TException), exception.InnerException.GetType());
            }
        }        

        private Exception NewInnerExceptionNotFoundException(Type expectedType, Type exceptionType)
        {
            var messageFormat = ExceptionMessages.Scenario_InnerExceptionNotFound;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), expectedType.FriendlyName());
            return NewAssertFailedException(message);
        }

        private Exception NewInnerExceptionOfDifferentTypeException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Scenario_InnerExceptionOfDifferentType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), actualType.FriendlyName());
            return NewAssertFailedException(message);
        }

        /// <summary>
        /// Creates and returns a new exception that indicates that an assertion has failed.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Optional cause of the exception.</param>
        /// <returns>
        /// A new exception that indicates that an assertion has failed with the specified <paramref name="message"/>
        /// and <paramref name="innerException"/> as inner exception.
        /// </returns>
        protected abstract Exception NewAssertFailedException(string message, Exception innerException = null);

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>    
    public abstract class ScenarioTest : AutomatedTest, IMessageStream
    {
        private readonly Lazy<IMicroProcessor> _processor;
        private readonly Lazy<IMessageStream> _stream;

        internal ScenarioTest()
        {
            _processor = new Lazy<IMicroProcessor>(CreateProcessor);
            _stream = new Lazy<IMessageStream>(CreateStream);
        }

        /// <summary>
        /// Returns the processor that is used to execute this <see cref="ScenarioTest{TResult}"/>.
        /// </summary>
        protected IMicroProcessor Processor =>
            _processor.Value;

        /// <summary>
        /// Creates and returns the processor that is used to execute this <see cref="ScenarioTest{TResult}" />.
        /// </summary>
        protected virtual IMicroProcessor CreateProcessor() =>
            new MicroProcessor();

        internal virtual IMessageStream CreateStream() =>
            Given().Join();

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()}: {Stream}";

        #region [====== IMessageStream ======]

        private IMessageStream Stream =>
            _stream.Value;

        /// <inheritdoc />
        public int Count =>
            Stream.Count;

        /// <inheritdoc />
        public object this[int index] =>
            Stream[index];

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator() =>
            Stream.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Stream.GetEnumerator();

        /// <inheritdoc />
        public IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler) =>
            Stream.Append(message, handler);

        /// <inheritdoc />
        public IMessageStream AppendStream(IMessageStream stream) =>
            Stream.AppendStream(stream);

        Task IMessageStream.HandleMessagesWithAsync(IMessageHandler handler) =>
            HandleMessagesWithAsync(handler);

        /// <summary>
        /// Lets the specified <paramref name="handler"/> handle all messages of this stream and returns a stream of events.
        /// </summary>
        /// <param name="handler">A handler of messages.</param>   
        /// <returns>A task representing the operation.</returns> 
        protected Task HandleMessagesWithAsync(IMessageHandler handler) =>
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
            Task.CompletedTask;

        #endregion

        #region [====== Given / When / Then ======]        

        /// <summary>
        /// Returns a collection of streams that are processed by the <see cref="Processor" /> to setup a specific domain state.
        /// </summary>
        /// <returns>A collection of streams resulting from all messages that were processed.</returns>
        protected virtual IEnumerable<IMessageStream> Given() =>
            Enumerable.Empty<IMessageStream>();        

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
        /// <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        protected void AssertInnerExceptionIsOfType<TException>(Exception exception, Action<TException> assertCallback = null) where TException : Exception
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (exception.InnerException == null)
            {
                throw NewInnerExceptionNotFoundException(typeof(TException), exception.GetType());
            }            
            if (TryGetInnerException(exception, out TException innerException))
            {
                assertCallback?.Invoke(innerException);
                return;
            }
            throw NewInnerExceptionOfDifferentTypeException(typeof(TException), exception.InnerException.GetType());
        }

        private static bool TryGetInnerException<TException>(Exception exception, out TException innerException) where TException : Exception
        {            
            try
            {
                innerException = (TException) exception.InnerException;
                return true;
            }
            catch (InvalidCastException)
            {
                innerException = null;
                return false;
            }
        }        

        private Exception NewInnerExceptionNotFoundException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Scenario_InnerExceptionNotFound;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), actualType.FriendlyName());
            return NewAssertFailedException(message);
        }

        private Exception NewInnerExceptionOfDifferentTypeException(Type expectedType, Type actualType)
        {
            var messageFormat = ExceptionMessages.Scenario_InnerExceptionOfDifferentType;
            var message = string.Format(messageFormat, expectedType.FriendlyName(), actualType.FriendlyName());
            return NewAssertFailedException(message);
        }        

        #endregion
    }
}

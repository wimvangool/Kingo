using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Clocks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>           
    public abstract class Scenario : MessageSequence, IExecutable
    {
        #region [====== Execution ======]

        /// <summary>
        /// Returns the processor that is used to execute this <see cref="Scenario" />.
        /// </summary>
        protected abstract IMessageProcessor MessageProcessor
        {
            get;
        }

        /// <inheritdoc />
        public void Execute()
        {
            ExecuteCoreAsync().Wait();
        }

        /// <inheritdoc />
        public virtual Task ExecuteAsync()
        {
            return ExecuteCoreAsync();
        }

        internal Task ExecuteCoreAsync()
        {
            return ProcessWithAsync(MessageProcessor);
        }        

        #endregion        

        #region [====== Test Value Randomization ======]

        private static readonly ThreadLocal<Random> _NumberGenerator = new ThreadLocal<Random>(() => new Random(), true);

        /// <summary>
        /// Returns a random number generator.
        /// </summary>
        protected static Random NumberGenerator
        {
            get { return _NumberGenerator.Value; }
        }

        /// <summary>
        /// Returns the current (local) date and time.
        /// </summary>
        /// <returns>The current date and time.</returns>
        protected static DateTimeOffset CurrentDateAndTime()
        {
            return Clock.Current.LocalDateAndTime();
        }

        /// <summary>
        /// Picks and returns a random value from the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the value of the collection.</typeparam>
        /// <param name="values">The collection of values to pick the value from.</param>
        /// <returns>One element of the specified collection of <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="values"/> does not contain any elements.
        /// </exception>
        protected static TValue PickValueFrom<TValue>(params TValue[] values)
        {
            return PickValueFrom(values.ToList() as IList<TValue>);
        }

        /// <summary>
        /// Picks and returns a random value from the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the value of the collection.</typeparam>
        /// <param name="values">The collection of values to pick the value from.</param>
        /// <returns>One element of the specified collection of <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="values"/> does not contain any elements.
        /// </exception>
        protected static TValue PickValueFrom<TValue>(IEnumerable<TValue> values)
        {
            return PickValueFrom(values.ToList() as IList<TValue>);
        }

        /// <summary>
        /// Picks and returns a random value from the specified collection.
        /// </summary>
        /// <typeparam name="TValue">Type of the value of the collection.</typeparam>
        /// <param name="values">The collection of values to pick the value from.</param>
        /// <returns>One element of the specified collection of <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="values"/> does not contain any elements.
        /// </exception>
        protected static TValue PickValueFrom<TValue>(IList<TValue> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (values.Count == 0)
            {
                throw NewEmptyCollectionException(typeof(TValue));
            }
            if (values.Count == 1)
            {
                return values[0];
            }
            return values[NumberGenerator.Next(0, values.Count)];
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static Exception NewEmptyCollectionException(Type valueType)
        {
            var messageFormat = ExceptionMessages.Scenario_EmptyCollectionSpecified;
            var message = string.Format(messageFormat, valueType.Name);
            return new ArgumentException(message, "values");
        }

        #endregion
    }
}

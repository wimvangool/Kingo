using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Syztem.Resources;
using Syztem.Threading;

namespace Syztem.ComponentModel.Server
{
    /// <summary>
    /// Represents a scenario that follows the Behavior Driven Development (BDD) style, which is characterized
    /// by the Given-When-Then pattern.
    /// </summary>           
    public abstract class Scenario : MessageSequence
    {
        #region [====== Execution ======]

        /// <summary>
        /// Returns the processor that is used to execute this <see cref="Scenario" />.
        /// </summary>
        protected abstract IMessageProcessor MessageProcessor
        {
            get;
        }        

        /// <summary>
        /// Executes this <see cref="Scenario" />.
        /// </summary>
        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        /// <summary>
        /// Executes this <see cref="Scenario" /> asynchronously.
        /// </summary>
        /// <returns>A task carrying out this operation.</returns>
        public async Task ExecuteAsync()
        {
            var previousCache = Cache;

            Cache = new DependencyCache();

            try
            {
                await ProcessWithAsync(MessageProcessor);
            }
            finally
            {
                Cache.Dispose();
                Cache = previousCache;
            } 
        }

        #endregion

        #region [====== Cache ======]

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly AsyncLocal<DependencyCache> _Cache = new AsyncLocal<DependencyCache>();

        /// <summary>
        /// Returns the <see cref="Scenario" /> that is currently being executed on this thread.
        /// </summary>
        internal static DependencyCache Cache
        {
            get {  return _Cache.Value; }
            private set { _Cache.Value = value; }
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
                throw NewEmptyCollectionException(values);
            }
            if (values.Count == 1)
            {
                return values[0];
            }
            return values[NumberGenerator.Next(0, values.Count)];
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        private static Exception NewEmptyCollectionException(object values)
        {
            var messageFormat = ExceptionMessages.Scenario_EmptyCollectionSpecified;
            var message = string.Format(messageFormat, values.GetType());
            return new ArgumentException(message, "values");
        }

        #endregion
    }
}

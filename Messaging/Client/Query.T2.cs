using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a query that can execute or dispatch itself, representing both a <see cref="Message{T}" /> and a <see cref="IQueryDispatcher{T}" />.
    /// </summary>
    /// <typeparam name="TQuery">Type of the specific query to implement.</typeparam>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class Query<TQuery, TResult> : Message<TQuery>, IQueryDispatcher<TResult>, IEquatable<TQuery>
        where TQuery : Query<TQuery, TResult>
    {        
        private readonly Lazy<IQueryDispatcher<TResult>> _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T, S}" /> class.
        /// </summary>
        protected Query()
        {
            _dispatcher = new Lazy<IQueryDispatcher<TResult>>(CreateDispatcherRelay);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <param name="makeReadOnly">Indicates wther or not this copy should be marked readonly.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Query(TQuery message, bool makeReadOnly)
            : base(message, makeReadOnly)
        {
            _dispatcher = message._dispatcher;
        }

        private IQueryDispatcher<TResult> CreateDispatcherRelay()
        {
            return new QueryDispatcherRelay<TQuery, TResult>((TQuery) this);
        }        

        #region [====== Execution ======]

        /// <summary>
        /// Returns the <see cref="IQueryDispatcher{T}" /> that is used internally to execute this query.
        /// </summary>
        protected IQueryDispatcher<TResult> Dispatcher
        {
            get { return _dispatcher.Value; }
        }

        private IRequestDispatcher RequestDispatcher
        {
            get { return _dispatcher.Value; }
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionSucceededEventArgs<TResult>> ExecutionSucceeded
        {
            add { Dispatcher.ExecutionSucceeded += value; }
            remove { Dispatcher.ExecutionSucceeded -= value; }
        }

        /// <inheritdoc />
        public TResult Execute()
        {
            return Dispatcher.Execute();
        }

        /// <inheritdoc />
        public TResult Execute(QueryCache cache)
        {
            return Dispatcher.Execute(cache);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync()
        {
            return Dispatcher.ExecuteAsync();
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(CancellationToken? token)
        {
            return Dispatcher.ExecuteAsync(token);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(QueryCache cache)
        {
            return Dispatcher.ExecuteAsync(cache);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteAsync(QueryCache cache, CancellationToken? token)
        {
            return Dispatcher.ExecuteAsync(cache, token);
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionStartedEventArgs> ExecutionStarted;

        /// <inheritdoc />
        public event EventHandler<ExecutionCanceledEventArgs> ExecutionCanceled;

        /// <inheritdoc />
        public event EventHandler<ExecutionFailedEventArgs> ExecutionFailed;

        /// <inheritdoc />
        event EventHandler<ExecutionSucceededEventArgs> IRequestDispatcher.ExecutionSucceeded
        {
            add { RequestDispatcher.ExecutionSucceeded += value; }
            remove { RequestDispatcher.ExecutionSucceeded -= value; }
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted;

        /// <inheritdoc />
        public event EventHandler IsExecutingChanged;

        /// <inheritdoc />
        public bool IsExecuting
        {
            get { return Dispatcher.IsExecuting; }
        }

        #endregion

        #region [====== Equality ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as TQuery);
        }

        /// <inheritdoc />
        public virtual bool Equals(TQuery other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return GetType() == other.GetType();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        #endregion

        #region [====== Implementation ======]

        protected internal abstract TResult Execute(TQuery query, CancellationToken? token);

        protected internal virtual QueryCacheValue CreateCacheValue(object key, TResult result)
        {
            return new QueryCacheValue(result);
        }

        #endregion
    }
}

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
        public event EventHandler<ExecutionStartedEventArgs> ExecutionStarted
        {
            add { Dispatcher.ExecutionStarted += value; }
            remove { Dispatcher.ExecutionStarted -= value; }
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionCanceledEventArgs> ExecutionCanceled
        {
            add { Dispatcher.ExecutionCanceled += value; }
            remove { Dispatcher.ExecutionCanceled -= value; }
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionFailedEventArgs> ExecutionFailed
        {
            add { Dispatcher.ExecutionFailed += value; }
            remove { Dispatcher.ExecutionFailed -= value; }
        }

        /// <inheritdoc />
        event EventHandler<ExecutionSucceededEventArgs> IRequestDispatcher.ExecutionSucceeded
        {
            add { RequestDispatcher.ExecutionSucceeded += value; }
            remove { RequestDispatcher.ExecutionSucceeded -= value; }
        }

        /// <inheritdoc />
        public event EventHandler<ExecutionCompletedEventArgs> ExecutionCompleted
        {
            add { Dispatcher.ExecutionCompleted += value; }
            remove { Dispatcher.ExecutionCompleted -= value; }
        }

        /// <inheritdoc />
        public event EventHandler IsExecutingChanged
        {
            add { Dispatcher.IsExecutingChanged += value; }
            remove { Dispatcher.IsExecutingChanged -= value; }
        }

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

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task{T}" /> that is used to execute this query.
        /// </summary>
        /// <param name="query">The action that will be invoked on the background thread.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory{T}.StartNew(Func{T})">StartNew</see>-method
        /// to start and return a new <see cref="Task{T}" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected internal virtual Task<TResult> Start(Func<TResult> query)
        {
            return Task<TResult>.Factory.StartNew(query);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>        
        /// <param name="query">The execution-parameter.</param>
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this query.
        /// </param>         
        /// <exception cref="OperationCanceledException">
        /// <paramref name="token"/> was specified and used to cancel the execution.
        /// </exception>        
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected internal abstract TResult Execute(TQuery query, CancellationToken? token);

        /// <summary>
        /// Wraps the specified result into a new <see cref="QueryCacheValue" />.
        /// </summary>
        /// <param name="key">The key that will be used to store this value.</param>
        /// <param name="result">The result returned by this query.</param>
        /// <returns>A new <see cref="QueryCacheValue" />.</returns>
        /// <remarks>
        /// By default, this method returns a value with an infinite lifetime. You may want to override this method
        /// to specify another lifetime for the cached value.
        /// </remarks>
        protected internal virtual QueryCacheValue CreateCacheValue(object key, TResult result)
        {
            return new QueryCacheValue(result);
        }

        #endregion
    }
}

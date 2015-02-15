using System.ComponentModel.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a task that can execute a query asynchronously.
    /// </summary>
    public class QueryExecutionTask<TMessageOut> : AsyncExecutionTask<IQueryDispatcher<TMessageOut>> where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IQueryDispatcher<TMessageOut> _dispatcher;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutionTask{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public QueryExecutionTask(IQueryDispatcher<TMessageOut> dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            _dispatcher = dispatcher;           
        }

        /// <inheritdoc />
        protected override IQueryDispatcher<TMessageOut> Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Gets or sets the <see cref="QueryExecutionOptions" /> of the query to execute.
        /// </summary>
        public QueryExecutionOptions ExecutionOptions
        {
            get;
            set;
        }

        /// <summary>
        /// If the task has been started, returns the associated <see cref="Task" /> instance of this task.
        /// </summary>
        protected Task<TMessageOut> Task
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override void Execute(CancellationToken token)
        {
            Task = _dispatcher.ExecuteAsync(RequestId, ExecutionOptions, token);
        }
    }
}

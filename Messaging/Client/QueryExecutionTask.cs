using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a task that can execute a query asynchronously.
    /// </summary>
    public class QueryExecutionTask<TResponse> : AsyncExecutionTask<IQueryDispatcher<TResponse>> where TResponse : IMessage
    {
        private readonly IQueryDispatcher<TResponse> _dispatcher;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutionTask{T}" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute the query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public QueryExecutionTask(IQueryDispatcher<TResponse> dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            _dispatcher = dispatcher;           
        }

        /// <inheritdoc />
        protected override IQueryDispatcher<TResponse> Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// If the task has been started, returns the associated <see cref="Task" /> instance of this task.
        /// </summary>
        protected Task<TResponse> Task
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override void Execute(CancellationToken token)
        {
            Task = _dispatcher.ExecuteAsync(RequestId, token);
        }
    }
}

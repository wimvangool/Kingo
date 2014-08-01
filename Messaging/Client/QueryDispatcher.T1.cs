﻿using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ComponentModel.Messaging.Client
{    
    /// <summary>
    /// Represents a command that has no execution-parameter(s).
    /// </summary>
    /// <typeparam name="TResult">Type of the result of this query.</typeparam>
    public abstract class QueryDispatcher<TResult> : QueryDispatcherBase<TResult>        
    {        
        #region [====== Execution ======]

        /// <inheritdoc />
        public override TResult Execute(QueryCache cache)
        {
            TransactionScope transactionScope = null;
            var executionId = Guid.NewGuid();            

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            try
            {
                transactionScope = CreateTransactionScope();

                result = ExecuteQuery(cache, null);

                transactionScope.Complete();
            }
            catch (Exception exception)
            {
                OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception));
                throw;
            }
            finally
            {
                if (transactionScope != null)
                {
                    transactionScope.Dispose();
                }
            }
            OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result));

            return result;
        }

        /// <inheritdoc />
        public override Task<TResult> ExecuteAsync(QueryCache cache, CancellationToken? token)
        {           
            var executionId = Guid.NewGuid();
            var context = SynchronizationContext.Current;

            OnExecutionStarted(new ExecutionStartedEventArgs(executionId));
            TResult result;

            if (QueryCache.TryGetFromCache(cache, GetType(), out result))
            {
                OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result));

                return CreateCompletedTask(result);
            }
            return Start(() =>
            {
                using (var scope = new SynchronizationContextScope(context))
                {
                    TransactionScope transactionScope = null;

                    try
                    {
                        token.ThrowIfCancellationRequested();

                        transactionScope = CreateTransactionScope();

                        result = ExecuteQuery(cache, token);                        

                        transactionScope.Complete();
                    }
                    catch (OperationCanceledException exception)
                    {
                        scope.Post(() => OnExecutionCanceled(new ExecutionCanceledEventArgs(executionId, exception)));
                        throw;
                    }
                    catch (Exception exception)
                    {
                        scope.Post(() => OnExecutionFailed(new ExecutionFailedEventArgs(executionId, exception)));
                        throw;
                    }
                    finally
                    {
                        if (transactionScope != null)
                        {
                            transactionScope.Dispose();
                        }
                    }
                    scope.Post(() => OnExecutionSucceeded(new ExecutionSucceededEventArgs<TResult>(executionId, result)));

                    return result;
                }
            });
        }

        /// <summary>
        /// Creates, starts and returns a new <see cref="Task{T}" /> that is used to execute this command.
        /// </summary>
        /// <param name="query">The action that will be invoked on the background thread.</param>
        /// <returns>The newly created task.</returns>
        /// <remarks>
        /// The default implementation uses the <see cref="TaskFactory{T}.StartNew(Func{T})">StartNew</see>-method
        /// to start and return a new <see cref="Task{T}" />. You may want to override this method to specify
        /// more options when creating this task.
        /// </remarks>
        protected virtual Task<TResult> Start(Func<TResult> query)
        {
            return Task<TResult>.Factory.StartNew(query);
        }

        private TResult ExecuteQuery(QueryCache cache, CancellationToken? token)
        {
            return cache == null
                ? Execute(token)
                : cache.GetOrAdd(GetType(), key => CreateCacheValue(key, Execute(token))).Access<TResult>();
        }

        /// <summary>
        /// Executes this query.
        /// </summary>        
        /// <returns>The result of this query.</returns>       
        /// <remarks>
        /// Note that this method may be invoked from any thread, so access to any shared resources must be thread-safe.
        /// </remarks>
        protected abstract TResult Execute(CancellationToken? token);

        #endregion        
    }
}

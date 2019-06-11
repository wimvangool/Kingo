﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>    
    /// Represents an operation of a <see cref="IMicroProcessor"/> where a query ie being executed.    
    /// </summary>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public abstract class ExecuteAsyncMethodOperation<TResponse> : MicroProcessorOperation<QueryOperationResult<TResponse>>, IAsyncMethodOperation<QueryOperationResult<TResponse>>
    {
        #region [====== IAsyncMethodOperation ======]

        IAsyncMethod IAsyncMethodOperation.Method =>
            Method;

        /// <summary>
        /// Returns the <see cref="IQuery{TResponse}.ExecuteAsync"/> or <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/>
        /// method that is being invoked in this operation.
        /// </summary>
        public abstract ExecuteAsyncMethod Method
        {
            get;
        }        

        MicroProcessorOperationContext IAsyncMethodOperation.Context =>
            Context;

        /// <summary>
        /// Returns the context of this operation.
        /// </summary>
        public abstract QueryOperationContext Context
        {
            get;
        }        

        #endregion

        #region [====== IMicroProcessorOperation<ExecuteAsyncResult<TResponse>> ======]

        /// <inheritdoc />
        public AsyncMethodOperation<QueryOperationResult<TResponse>> ToAsyncMethodOperation() =>
            new AsyncMethodOperation<QueryOperationResult<TResponse>>(this);        

        #endregion
    }
}

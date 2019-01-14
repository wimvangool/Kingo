using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base class for all filters that can be part of the <see cref="IMicroProcessor" /> pipeline.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class MicroProcessorFilterAttribute : Attribute, IMicroProcessorFilter
    {                     
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorFilterAttribute" /> class.
        /// </summary>
        internal MicroProcessorFilterAttribute(MicroProcessorFilterStage stage)
        {
            Stage = stage;
            OperationTypes = MicroProcessorOperationTypes.All;
        }

        /// <inheritdoc />
        public MicroProcessorFilterStage Stage
        {
            get;
        }

        /// <summary>                 
        /// Indicates for which operations this filter will be used.
        /// </summary>
        public MicroProcessorOperationTypes OperationTypes
        {
            get;
            set;
        }        

        /// <inheritdoc />
        public virtual bool IsEnabled(MicroProcessorContext context) =>
            OperationTypes.HasFlag(context.Operation.Type);                                    

        #region [====== IMicroProcessorFilter ======]                                

        /// <inheritdoc /> 
        public virtual Task<InvokeAsyncResult<MessageStream>> InvokeMessageHandlerAsync(MessageHandler handler) =>
            InvokeAsync(handler);

        /// <inheritdoc />
        public virtual Task<InvokeAsyncResult<TResponse>> InvokeQueryAsync<TResponse>(Query<TResponse> query) =>
            InvokeAsync(query);        

        /// <summary>
        /// Invokes the specified <paramref name="handlerOrQuery"/> and returns it result.
        /// </summary>        
        /// <typeparam name="TResult">Type of the result of the operation.</typeparam>
        /// <param name="handlerOrQuery">A message handler or query that will perform the operation.</param>       
        /// <returns>The result of the operation.</returns>
        protected virtual Task<InvokeAsyncResult<TResult>> InvokeAsync<TResult>(IMessageHandlerOrQuery<TResult> handlerOrQuery) =>
            handlerOrQuery.Method.InvokeAsync();

        #endregion        
    }
}

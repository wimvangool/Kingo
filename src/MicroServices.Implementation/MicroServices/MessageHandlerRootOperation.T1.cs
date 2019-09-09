using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal abstract class MessageHandlerRootOperation<TMessage> : MessageHandlerOperation<TMessage>
    {
        private readonly HandleAsyncMethod<TMessage> _method;
        private readonly IUnitOfWork _unitOfWork;

        protected MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, IMessageToProcess<TMessage> message, CancellationToken? token) :
            this(processor, method, message, token, UnitOfWork.InMode(processor.Options.UnitOfWorkMode)) { }
            
        private MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, IMessageToProcess<TMessage> message, CancellationToken? token, IUnitOfWork unitOfWork) :
            base(new MessageHandlerOperationContext(processor, unitOfWork), message, token)
        {
            _method = method;
            _unitOfWork = unitOfWork;
        }

        public override MicroProcessorOperationKind Kind =>
            MicroProcessorOperationKind.RootOperation;      

        public override async Task<MessageHandlerOperationResult> ExecuteAsync()
        {
            try
            {
                // After the logical transaction has been completed, all changes are flushed.
                var result = await base.ExecuteAsync().ConfigureAwait(false);
                await _unitOfWork.FlushAsync().ConfigureAwait(false);
                return result;
            }
            catch (MessageHandlerOperationException exception)
            {
                // Depending on whether this operation is executing a command or handling an event,
                // the exception is converted to the appropriate MicroProcessorOperationException.
                throw NewMicroProcessorOperationException(exception);
            }            
            catch (MicroProcessorOperationException)
            {
                // MicroProcessorOperations are left through by default, because these exceptions
                // contain the error information that the client of the processor will want to use
                // to properly handle the exception.
                throw;
            }
            catch (OperationCanceledException exception)
            {
                // OperationCanceledExceptions are left through if and only if they were thrown because
                // cancellation of the processor operation was requested. In any other case they are regarded
                // as regular unhandled exceptions that represent an error.
                if (exception.CancellationToken == Token)
                {
                    throw;
                }                             
                throw InternalServerErrorException.FromInnerException(exception);                
            }
            catch (Exception exception)
            {
                throw InternalServerErrorException.FromInnerException(exception);
            }
        }

        protected abstract MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException exception);        

        protected override IEnumerable<HandleAsyncMethodOperation> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            yield return CreateMethodOperation(_method, context);
        }                 
    }
}

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

        protected MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, MessageToProcess<TMessage> message, CancellationToken? token) :
            this(processor, method, message, token, UnitOfWork.InMode(processor.Options.UnitOfWorkMode)) { }
            
        private MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, MessageToProcess<TMessage> message, CancellationToken? token, IUnitOfWork unitOfWork) :
            base(new MessageHandlerOperationContext(processor, unitOfWork), message, token)
        {
            _method = method;
            _unitOfWork = unitOfWork;
        }

        public override MicroProcessorOperationKind Kind =>
            MicroProcessorOperationKind.RootOperation;      

        protected override async Task<MessageHandlerOperationResult> ExecuteAsync(HandleAsyncMethodOperation operation)
        {
            try
            {
                // After the logical transaction has been completed, all changes are flushed.
                var result = await base.ExecuteAsync(operation).ConfigureAwait(false);
                await _unitOfWork.SaveChangesAsync().ConfigureAwait(false);
                return result;
            }
            catch (MicroProcessorOperationException)
            {
                // MicroProcessorOperations are let through by default, because these exceptions
                // contain the error information that the client of the processor will want to use
                // to properly handle the exception.
                throw;
            }
            catch (MessageHandlerOperationException exception)
            {
                throw NewMicroProcessorOperationException(exception.AssignStackTrace(operation.Context.CaptureOperationStackTrace()));
            }
            catch (MessageHandlerOperationException.WithStackTrace exception)
            {
                throw NewMicroProcessorOperationException(exception);
            }
            catch (Exception exception)
            {
                throw operation.Context.NewInternalServerErrorException(ExceptionMessages.MicroProcessorOperation_InternalServerError, exception);
            }
        }

        // Depending on whether this operation is executing a command or handling an event,
        // the exception is converted to the appropriate MicroProcessorOperationException.
        protected abstract MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException.WithStackTrace exception);        

        protected override IEnumerable<HandleAsyncMethodOperation> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            yield return CreateMethodOperation(_method, context);
        }                 
    }
}

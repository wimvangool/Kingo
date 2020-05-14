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

        protected MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, Message<TMessage> message, CancellationToken? token) :
            this(processor, method, message, token, UnitOfWork.InMode(processor.Settings.UnitOfWorkMode)) { }
            
        private MessageHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TMessage> method, Message<TMessage> message, CancellationToken? token, IUnitOfWork unitOfWork) :
            base(new MessageHandlerOperationContext(processor, unitOfWork), message, token)
        {
            _method = method;
            _unitOfWork = unitOfWork;
        }

        public override MicroProcessorOperationKind Kind =>
            MicroProcessorOperationKind.RootOperation;      

        internal override async Task<MessageHandlerOperationResult<TMessage>> ExecuteAsync(HandleAsyncMethodOperation<TMessage> operation)
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
            catch (InternalOperationException exception)
            {
                throw exception.ToMicroProcessorOperationException(operation.Context.CaptureOperationStackTrace());
            }
            catch (Exception exception)
            {
                throw operation.Context.NewInternalServerErrorException(ExceptionMessages.MicroProcessorOperation_InternalServerError, exception);
            }
        }

        protected override IEnumerable<HandleAsyncMethodOperation<TMessage>> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            yield return CreateMethodOperation(_method, context);
        }                 
    }
}

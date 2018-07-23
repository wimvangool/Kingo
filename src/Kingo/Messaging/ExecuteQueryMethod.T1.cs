using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a method that executes a query and returns its result.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
    public abstract class ExecuteQueryMethod<TMessageOut> : MicroProcessorMethod<TMessageOut>
    {
        private readonly MicroProcessor _processor;
        private readonly QueryContext _context;

        internal ExecuteQueryMethod(MicroProcessor processor, CancellationToken? token)
        {
            _processor = processor;
            _context = new QueryContext(processor.Principal, token);
        }

        /// <inheritdoc />
        public override CancellationToken Token =>
            _context.Token;        

        /// <summary>
        /// Invokes the query and returns its result.
        /// </summary>
        /// <returns>The result of the query.</returns>
        public override async Task<TMessageOut> InvokeAsync()
        {
            using (MicroProcessorContext.CreateScope(_context))
            {
                return await InvokeQueryAsync();
            }                     
        }           

        private async Task<TMessageOut> InvokeQueryAsync()
        {
            _context.Token.ThrowIfCancellationRequested();

            try
            {
                return (await InvokeQueryAsync(_processor, _context)).Value;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ExternalProcessorException)
            {
                throw;
            }
            catch (InternalProcessorException exception)
            {
                throw exception.AsBadRequestException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new InternalServerErrorException(ExceptionMessages.ExecuteAsyncMethod_InternalServerError, exception);
            }          
            finally
            {
                _context.Token.ThrowIfCancellationRequested();
            }
        }

        internal abstract Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync(MicroProcessor processor, QueryContext context);        
    }
}

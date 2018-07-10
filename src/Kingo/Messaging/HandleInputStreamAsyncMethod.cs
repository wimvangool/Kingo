using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class HandleInputStreamAsyncMethod : HandleStreamAsyncMethod
    {
        public static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, CancellationToken? token)
        {
            var message = inputStream[0];

            try
            {
                return await Invoke(processor, inputStream, new MessageHandlerContext(processor.Principal, token));
            }
            catch (OperationCanceledException)
            {                
                throw;
            }
            catch (ExternalProcessorException)
            {                
                throw;
            }            
            catch (ConcurrencyException exception)
            {
                // Only commands should promote concurrency-exceptions to conflict-exceptions.
                if (processor.IsCommand(message))
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (Exception exception)
            {                
                throw InternalServerErrorException.FromInnerException(exception);
            }
        }

        private static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, MessageHandlerContext context)
        {
            using (var scope = MicroProcessorContext.CreateScope(context))
            {
                var method = new HandleInputStreamAsyncMethod(processor, context);

                try
                {
                    await inputStream.HandleMessagesWithAsync(method);
                }
                finally
                {
                    await Task.WhenAll(method._handleMetadataStreamMethods);
                }                
                await scope.CompleteAsync();

                return method._outputStream;
            }
        }

        private readonly List<Task> _handleMetadataStreamMethods;        
        private IMessageStream _outputStream;

        private HandleInputStreamAsyncMethod(MicroProcessor processor, MessageHandlerContext context)
        {
            Processor = processor;
            Context = context;           

            _handleMetadataStreamMethods = new List<Task>();    
            _outputStream = MessageStream.Empty;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override MessageHandlerContext Context
        {
            get;
        }             

        protected override async Task HandleAsyncCore<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            // Every event that is published to the context's OutputStream is added to the final output stream.
            if (Context.Messages.Current.Source == MessageSources.OutputStream)
            {
                _outputStream = _outputStream.Append(message);
            }
            try
            {
                await base.HandleAsyncCore(message, handler);
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
                if (IsCommand(message))
                {
                    throw NewCommandHandlerException(message, exception);
                }
                throw NewEventHandlerException(message, exception);
            }
            catch (Exception exception)
            {                
                throw InternalServerErrorException.FromInnerException(exception);
            }            
        }

        private bool IsCommand(object message) =>
            Context.Messages.Current.Source == MessageSources.InputStream && Processor.IsCommand(message);

        private static BadRequestException NewCommandHandlerException(object command, InternalProcessorException exception)
        {
            var messageFormat = ExceptionMessages.HandleInputStreamAsyncMethod_CommandHandlerException;
            var message = string.Format(messageFormat, command.GetType().FriendlyName());
            return exception.AsBadRequestException(message);
        }                      

        protected override MessageInfo CreateMessageInfo(object message) =>
            Context.Messages.IsEmpty ? MessageInfo.FromInputStream(message) : MessageInfo.FromOutputStream(message);        

        protected override async Task HandleStreamsAsync(IMessageStream metadataStream, IMessageStream outputStream)
        {            
            if (metadataStream.Count > 0)
            {                
                // The metadata stream is handled 'out-of-band' in order to optimize the performance.
                _handleMetadataStreamMethods.Add(HandleMetadataStreamAsyncMethod.Invoke(Processor, Context, metadataStream));
            }
            if (outputStream.Count > 0)
            {
                await outputStream.HandleMessagesWithAsync(this);
            }
        }                                
    }
}

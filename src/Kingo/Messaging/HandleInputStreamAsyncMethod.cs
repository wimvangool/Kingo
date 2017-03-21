using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class HandleInputStreamAsyncMethod : HandleStreamAsyncMethod
    {
        public static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, CancellationToken? token)
        {            
            var context = new MessageHandlerContext(token);

            using (var scope = MicroProcessorContext.CreateContextScope(context))
            {
                var method = new HandleInputStreamAsyncMethod(processor, context);

                await inputStream.HandleMessagesWithAsync(method);
                await Task.WhenAll(method._handleMetadataStreamMethods);
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
            catch (MicroProcessorException)
            {
                // When a MicroProcessorException is thrown, it was probably throw by this method
                // for a down stream message and can therefore just be rethrown here.
                throw;
            }
            catch (InternalProcessorException exception)
            {
                // When an InternalProcessorException is thrown, it must be determined whether
                // the current message is a Command or an Event. If it is a Command, then the cause
                // of the exception is a bad request; if is an event, it is an internal server error.                        
                if (IsCommand(message))
                {
                    throw NewCommandHandlerException(message, exception);
                }
                throw NewEventHandlerException(message, exception);
            }
            catch (Exception exception)
            {
                // Any exceptions other than MicroProcessor- and InternalProcessorExceptions are unexpected by
                // definition and can therefore be rethrown as InternalServerErrorExceptions immediately.
                throw InternalServerErrorException.FromInnerException(message, exception);
            }            
        }

        private bool IsCommand(object message) =>
            Context.Messages.Current.Source == MessageSources.InputStream && Processor.IsCommand(message);

        private static BadRequestException NewCommandHandlerException(object command, InternalProcessorException exception)
        {
            var messageFormat = ExceptionMessages.HandleInputStreamAsyncMethod_CommandHandlerException;
            var message = string.Format(messageFormat, command.GetType().FriendlyName());
            return exception.AsBadRequestException(command, message);
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

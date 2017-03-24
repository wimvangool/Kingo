using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class HandleMetadataStreamAsyncMethod : HandleStreamAsyncMethod
    {
        public static Task Invoke(MicroProcessor processor, MicroProcessorContext context, IMessageStream metadataStream) =>
            processor.HandleMetadataStreamAsync(metadataStream, new HandleMetadataStreamAsyncMethod(processor, context.CreateMetadataContext()).Run);

        private HandleMetadataStreamAsyncMethod(MicroProcessor processor, MessageHandlerContext context)
        {
            Processor = processor;
            Context = context;            
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override MessageHandlerContext Context
        {
            get;
        }

        private Task Run(IMessageStream metadataStream)
        {
            return Task.Run(async () =>
            {
                var message = metadataStream[0];

                try
                {
                    await RunCore(metadataStream);
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
                    throw exception.AsInternalServerErrorException(message, exception.Message);
                }
                catch (Exception exception)
                {
                    throw InternalServerErrorException.FromInnerException(message, exception);
                }
            }, Context.Token);
        }

        private async Task RunCore(IMessageStream metadataStream)
        {
            using (var scope = MicroProcessorContext.CreateContextScope(Context))
            {
                await metadataStream.HandleMessagesWithAsync(this);
                await scope.CompleteAsync();
            }
        }

        protected override async Task HandleAsyncCore<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {            
            try
            {
                await base.HandleAsyncCore(message, handler);
            }
            catch (ExternalProcessorException)
            {
                // When a MicroProcessorException is thrown, it was probably throw by this method
                // for a down stream message and can therefore just be rethrown here.
                throw;
            }
            catch (InternalProcessorException exception)
            {                
                throw NewEventHandlerException(message, exception);
            }
            catch (Exception exception)
            {
                // Any exceptions other than MicroProcessor- and InternalProcessorExceptions are unexpected by
                // definition and can therefore be rethrown as InternalServerErrorExceptions immediately.
                throw InternalServerErrorException.FromInnerException(message, exception);
            }
        }

        protected override MessageInfo CreateMessageInfo(object message) =>
            MessageInfo.FromMetadataStream(message);

        protected override async Task HandleStreamsAsync(IMessageStream metadataStream, IMessageStream outputStream)
        {
            if (metadataStream.Count > 0)
            {
                await metadataStream.HandleMessagesWithAsync(this);
            }
        }
    }
}

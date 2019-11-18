﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all test-classes that execute tests based on test scenarios.
    /// </summary>
    public abstract class MicroProcessorOperationTestRunner : IMessageHandlerOperationTestProcessor
    {
        private readonly Lazy<IServiceProvider> _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorOperationTestRunner" /> class.
        /// </summary>
        protected MicroProcessorOperationTestRunner()
        {
            _serviceProvider = new Lazy<IServiceProvider>(CreateServiceProvider);
        }

        #region [====== ServiceProvider ======]

        /// <summary>
        /// Returns the <see cref="IServiceProvider" /> used by the <see cref="IMicroProcessor" />.
        /// </summary>
        protected IServiceProvider ServiceProvider =>
            _serviceProvider.Value;

        private IServiceProvider CreateServiceProvider() =>
            ConfigureServices(new ServiceCollection()).BuildServiceProvider(true);

        /// <summary>
        /// When overridden, configures <paramref name="services"/> with a <see cref="IMicroProcessor" /> and other
        /// dependencies for the tests to run.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        protected abstract IServiceCollection ConfigureServices(IServiceCollection services);

        #endregion

        #region [====== TestEngine ======]

        private abstract class TestEngine<TMicroProcessorTest> where TMicroProcessorTest : IMicroProcessorOperationTest
        {
            protected TestEngine(MicroProcessorOperationTestRunner testRunner, TMicroProcessorTest test, MicroProcessorOperationTestContext context)
            {
                TestRunner = testRunner;
                Test = test;
                Context = context;
                Result = new NullTestResult(test);
            }

            private MicroProcessorOperationTestRunner TestRunner
            {
                get;
            }

            protected TMicroProcessorTest Test
            {
                get;
            }

            protected MicroProcessorOperationTestContext Context
            {
                get;
            }

            protected IRunTestResult Result
            {
                get;
                set;
            }

            public async Task RunTestAsync()
            {                
                try
                {
                    await GivenAsync().ConfigureAwait(false);
                    await WhenAsync().ConfigureAwait(false);
                }
                catch (TestFailedException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Result = new UnexpectedExceptionResult(Test, exception);
                }
                finally
                {
                    Result.Complete();
                }
            }       
            
            private Task GivenAsync() =>
                Test.GivenAsync(TestRunner, Context);

            protected abstract Task WhenAsync();            
        }

        #endregion

        #region [====== RunAsync - MessageHandlerOperationTest ======]
        
        private sealed class MessageHandlerOperationTestEngine<TMessage, TMessageStream> : TestEngine<IMessageHandlerOperationTest<TMessage, TMessageStream>>, IMessageProcessor<TMessage> where TMessageStream : MessageStream
        {                      
            public MessageHandlerOperationTestEngine(MicroProcessorOperationTestRunner testRunner, IMessageHandlerOperationTest<TMessage, TMessageStream> test, MicroProcessorOperationTestContext context) :
                base(testRunner, test, context) { }

            protected override Task WhenAsync() =>
                Test.WhenAsync(this, Context);

            public Task ExecuteCommandAsync<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
                ExecuteCommandAsync(Context.ServiceProvider.GetRequiredService<TMessageHandler>(), message);

            public Task ExecuteCommandAsync(IMessageHandler<TMessage> messageHandler, TMessage message) =>
                ExecuteAsync(new ExecuteCommandOperation<TMessage, TMessageStream>(messageHandler, message));

            public Task HandleEventAsync<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage> =>
                HandleEventAsync(Context.ServiceProvider.GetRequiredService<TMessageHandler>(), message);

            public Task HandleEventAsync(IMessageHandler<TMessage> messageHandler, TMessage message) =>
                ExecuteAsync(new HandleEventOperation<TMessage, TMessageStream>(messageHandler, message));

            private async Task ExecuteAsync(HandleMessageOperation<TMessage, TMessageStream> operation)
            {
                var result = await operation.ExecuteAsync(Test, Context).ConfigureAwait(false);

                try
                {
                    Test.Then(operation.Message, result, Context);
                }
                finally
                {
                    Result = result;
                }
            }                       
        }

        private abstract class HandleMessageOperation<TMessage, TMessageStream>
            where TMessageStream : MessageStream
        {                                    
            public abstract TMessage Message
            {
                get;
            }            

            public async Task<MessageHandlerOperationTestResult<TMessageStream>> ExecuteAsync(IMicroProcessorOperationTest test, MicroProcessorOperationTestContext context)
            {
                MessageHandlerOperationResult<TMessage> result;

                try
                {
                    result = await HandleMessageAsync(context.Processor).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    return new MessageHandlerOperationTestResult<TMessageStream>(exception);
                }
                return new MessageHandlerOperationTestResult<TMessageStream>(new MessageStream(result.Output), stream =>
                {
                    context.SetOutputStream(test, stream);
                });
            }

            protected abstract Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync(IMicroProcessor processor);
        }

        private sealed class ExecuteCommandOperation<TMessage, TMessageStream> : HandleMessageOperation<TMessage, TMessageStream>
            where TMessageStream : MessageStream
        {
            private readonly IMessageHandler<TMessage> _messageHandler;
            private readonly TMessage _message;

            public ExecuteCommandOperation(IMessageHandler<TMessage> messageHandler, TMessage message)
            {
                _messageHandler = messageHandler;
                _message = message;
            }

            public override TMessage Message =>
                _message;

            protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync(IMicroProcessor processor) =>
                processor.ExecuteCommandAsync(_messageHandler, _message);
        }

        private sealed class HandleEventOperation<TMessage, TMessageStream> : HandleMessageOperation<TMessage, TMessageStream>
            where TMessageStream : MessageStream
        {
            private readonly IMessageHandler<TMessage> _messageHandler;
            private readonly TMessage _message;

            public HandleEventOperation(IMessageHandler<TMessage> messageHandler, TMessage message)
            {
                _messageHandler = messageHandler;
                _message = message;
            }

            public override TMessage Message =>
                _message;

            protected override Task<MessageHandlerOperationResult<TMessage>> HandleMessageAsync(IMicroProcessor processor) =>
                processor.HandleEventAsync(_messageHandler, _message);
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual async Task RunAsync<TMessage, TMessageStream>(IMessageHandlerOperationTest<TMessage, TMessageStream> test) where TMessageStream : MessageStream
        {            
            using (var scope = ServiceProvider.CreateScope())
            {
                await RunAsync(test, CreateTestContext(scope.ServiceProvider)).ConfigureAwait(false);
            }
        }        

        Task IMessageHandlerOperationTestProcessor.ExecuteCommandAsync<TMessage>(IMessageHandler<TMessage> messageHandler, TMessage message, MicroProcessorOperationTestContext context) =>
            RunAsync(new CommandHandlerOperationTestStub<TMessage>(messageHandler, message), context);

        Task IMessageHandlerOperationTestProcessor.HandleEventAsync<TMessage>(IMessageHandler<TMessage> messageHandler, TMessage message, MicroProcessorOperationTestContext context) =>
            RunAsync(new EventHandlerOperationTestStub<TMessage>(messageHandler, message), context);

        Task IMessageHandlerOperationTestProcessor.RunAsync<TMessage, TMessageStream>(IMessageHandlerOperationTest<TMessage, TMessageStream> test, MicroProcessorOperationTestContext context) =>
            RunAsync(test, context);

        private Task RunAsync<TMessage, TMessageStream>(IMessageHandlerOperationTest<TMessage, TMessageStream> test, MicroProcessorOperationTestContext context) where TMessageStream : MessageStream =>
            new MessageHandlerOperationTestEngine<TMessage, TMessageStream>(this, test, context).RunTestAsync();            

        #endregion

        #region [====== RunAsync - QueryOperationTest (1) ======]

        private sealed class QueryOperationTestEngine<TResponse> : TestEngine<IReadOperationTest<TResponse>>, IQueryOperationTestProcessor<TResponse>
        {           
            public QueryOperationTestEngine(MicroProcessorOperationTestRunner testRunner, IReadOperationTest<TResponse> test, MicroProcessorOperationTestContext context) :
                base(testRunner, test, context) { }

            protected override Task WhenAsync() =>
                Test.WhenAsync(this, Context);

            public async Task ExecuteAsync(IQuery<TResponse> query)
            {
                var result = await ExecuteQueryAsync(query, Context.Processor).ConfigureAwait(false);

                try
                {
                    Test.Then(result, Context);
                }                
                finally
                {
                    Result = result;
                }                
            }

            private static async Task<QueryOperationTestResult<TResponse>> ExecuteQueryAsync(IQuery<TResponse> query, IMicroProcessor processor)
            {
                try
                {
                    return new QueryOperationTestResult<TResponse>(await processor.ExecuteQueryAsync(query).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return new QueryOperationTestResult<TResponse>(exception);
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>            
        protected virtual async Task RunAsync<TResponse>(IReadOperationTest<TResponse> test)
        {            
            using (var scope = ServiceProvider.CreateScope())
            {
                await RunAsync(test, CreateTestContext(scope.ServiceProvider)).ConfigureAwait(false);
            }
        }
         
        private Task RunAsync<TResponse>(IReadOperationTest<TResponse> test, MicroProcessorOperationTestContext context) =>
            new QueryOperationTestEngine<TResponse>(this, test, context).RunTestAsync();

        #endregion

        #region [====== RunAsync - QueryOperationTest (2) ======]

        private sealed class QueryOperationTestEngine<TRequest, TResponse> : TestEngine<IQueryOperationTest<TRequest, TResponse>>, IQueryOperationTestProcessor<TRequest, TResponse>
        {           
            public QueryOperationTestEngine(MicroProcessorOperationTestRunner testRunner, IQueryOperationTest<TRequest, TResponse> test, MicroProcessorOperationTestContext context) :
                base(testRunner, test, context) { }

            protected override Task WhenAsync() =>
                Test.WhenAsync(this, Context);

            public async Task ExecuteAsync(IQuery<TRequest, TResponse> query, TRequest request)
            {
                var result = await ExecuteQueryAsync(request, query, Context.Processor).ConfigureAwait(false);

                try
                {
                    Test.Then(request, result, Context);
                }
                finally
                {
                    Result = result;
                }                                
            }

            private static async Task<QueryOperationTestResult<TResponse>> ExecuteQueryAsync(TRequest request, IQuery<TRequest, TResponse> query, IMicroProcessor processor)
            {
                try
                {
                    return new QueryOperationTestResult<TResponse>(await processor.ExecuteQueryAsync(query, request).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return new QueryOperationTestResult<TResponse>(exception);
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual async Task RunAsync<TRequest, TResponse>(IQueryOperationTest<TRequest, TResponse> test)
        {            
            using (var scope = ServiceProvider.CreateScope())
            {
                await RunAsync(test, CreateTestContext(scope.ServiceProvider)).ConfigureAwait(false);
            }
        }
        
        private Task RunAsync<TRequest, TResponse>(IQueryOperationTest<TRequest, TResponse> test, MicroProcessorOperationTestContext context) =>
            new QueryOperationTestEngine<TRequest, TResponse>(this, test, context).RunTestAsync();

        #endregion   

        private static MicroProcessorOperationTestContext CreateTestContext(IServiceProvider serviceProvider) =>
            new MicroProcessorOperationTestContext(ResolveProcessor(serviceProvider));
        
        private static IMicroProcessor ResolveProcessor(IServiceProvider serviceProvider)
        {
            var processor = serviceProvider.GetService<IMicroProcessor>();
            if (processor == null)
            {
                return new MicroProcessor(serviceProvider);
            }
            return processor;
        }
    }
}

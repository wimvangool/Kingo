using System;
using System.Threading.Tasks;
using Kingo.MicroServices.Configuration;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test-classes that execute tests based on test's.
    /// </summary>
    public abstract class MicroProcessorTestRunner : IMessageHandlerTestProcessor
    {        
        #region [====== RunAsync - HandleMessageTest ======]
        
        private sealed class HandleMessageTestRunner<TMessage, TEventStream> : IMessageProcessor<TMessage> where TEventStream : EventStream
        {
            private readonly MicroProcessorTestRunner _testRunner;
            private readonly IMessageHandlerTest<TMessage, TEventStream> _test;
            private readonly MicroProcessorTestContext _context;            

            public HandleMessageTestRunner(MicroProcessorTestRunner testRunner, IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context)
            {
                _testRunner = testRunner;
                _test = test;
                _context = context;
            }

            public async Task RunAsync()
            {
                await _test.GivenAsync(_testRunner, _context).ConfigureAwait(false);
                await _test.WhenAsync(this, _context).ConfigureAwait(false);
            }

            public Task HandleAsync(TMessage message, Func<TMessage, MessageHandlerContext, Task> handler) =>
                HandleAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler));

            public async Task HandleAsync(TMessage message, IMessageHandler<TMessage> handler = null) =>
               await HandleAsync(message, handler, _MicroProcessor.ResolveProcessor()).ConfigureAwait(false);

            private async Task HandleAsync(TMessage message, IMessageHandler<TMessage> handler, IMicroProcessor processor) =>
                _test.Then(message, await HandleMessageAsync(message, handler, processor).ConfigureAwait(false), _context);

            private async Task<IMessageHandlerResult<TEventStream>> HandleMessageAsync(TMessage message, IMessageHandler<TMessage> handler, IMicroProcessor processor)
            {
                try
                {
                    await processor.HandleAsync(message, handler).ConfigureAwait(false);

                    return new MessageHandlerResult<TEventStream>(_context.CommitEventStream(), stream =>
                    {
                        _context.SetEventStream(_test, stream);
                    });
                }
                catch (Exception exception)
                {
                    return new MessageHandlerResult<TEventStream>(exception);
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual async Task RunAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ServiceProvider()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }

        Task IMessageHandlerTestProcessor.HandleAsync<TMessage>(TMessage message, MicroProcessorTestContext context, Func<TMessage, MessageHandlerContext, Task> handler) =>
            RunAsync(new MessageHandlerTestStub<TMessage>(message, handler), context);

        Task IMessageHandlerTestProcessor.HandleAsync<TMessage>(TMessage message, MicroProcessorTestContext context, IMessageHandler<TMessage> handler) =>
            RunAsync(new MessageHandlerTestStub<TMessage>(message, handler), context);

        Task IMessageHandlerTestProcessor.HandleAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context) =>
            RunAsync(test, context);

        private Task RunAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context) where TEventStream : EventStream =>
            new HandleMessageTestRunner<TMessage, TEventStream>(this, test, context).RunAsync();            

        #endregion

        #region [====== RunAsync - ExecuteQueryTest (1) ======]

        private sealed class ExecuteQueryTestRunner<TResponse> : IQueryProcessor<TResponse>
        {
            private readonly MicroProcessorTestRunner _testRunner;
            private readonly IQueryTest<TResponse> _test;
            private readonly MicroProcessorTestContext _context;

            public ExecuteQueryTestRunner(MicroProcessorTestRunner testRunner, IQueryTest<TResponse> test, MicroProcessorTestContext context)
            {
                _testRunner = testRunner;
                _test = test;
                _context = context;
            }

            public async Task RunAsync()
            {
                await _test.GivenAsync(_testRunner, _context).ConfigureAwait(false);
                await _test.WhenAsync(this, _context).ConfigureAwait(false);
            }            

            public Task ExecuteAsync(Func<QueryContext, Task<TResponse>> query) =>
                ExecuteAsync(QueryDecorator<TResponse>.Decorate(query));

            public async Task ExecuteAsync(IQuery<TResponse> query) =>
                await ExecuteAsync(query, _MicroProcessor.ResolveProcessor()).ConfigureAwait(false);

            private async Task ExecuteAsync(IQuery<TResponse> query, IMicroProcessor processor) =>
                _test.Then(await ExecuteQueryAsync(query, processor).ConfigureAwait(false), _context);

            private static async Task<IQueryResult<TResponse>> ExecuteQueryAsync(IQuery<TResponse> query, IMicroProcessor processor)
            {
                try
                {
                    return new QueryResult<TResponse>(await processor.ExecuteAsync(query).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return new QueryResult<TResponse>(exception);
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>            
        protected virtual async Task RunAsync<TResponse>(IQueryTest<TResponse> test)
        {
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ServiceProvider()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }
         
        private Task RunAsync<TResponse>(IQueryTest<TResponse> test, MicroProcessorTestContext context) =>
            new ExecuteQueryTestRunner<TResponse>(this, test, context).RunAsync();

        #endregion

        #region [====== RunAsync - ExecuteQueryTest (2) ======]

        private sealed class ExecuteQueryTestRunner<TRequest, TResponse> : IQueryProcessor<TRequest, TResponse>
        {
            private readonly MicroProcessorTestRunner _testRunner;
            private readonly IQueryTest<TRequest, TResponse> _test;
            private readonly MicroProcessorTestContext _context;

            public ExecuteQueryTestRunner(MicroProcessorTestRunner testRunner, IQueryTest<TRequest, TResponse> test, MicroProcessorTestContext context)
            {
                _testRunner = testRunner;
                _test = test;
                _context = context;
            }

            public async Task RunAsync()
            {
                await _test.GivenAsync(_testRunner, _context).ConfigureAwait(false);
                await _test.WhenAsync(this, _context).ConfigureAwait(false);
            }

            public Task ExecuteAsync(TRequest request, Func<TRequest, QueryContext, Task<TResponse>> query) =>
                ExecuteAsync(request, QueryDecorator<TRequest, TResponse>.Decorate(query));

            public async Task ExecuteAsync(TRequest request, IQuery<TRequest, TResponse> query) =>
                await ExecuteAsync(request, query, _MicroProcessor.ResolveProcessor()).ConfigureAwait(false);

            private async Task ExecuteAsync(TRequest request, IQuery<TRequest, TResponse> query, IMicroProcessor processor) =>
                _test.Then(request, await ExecuteQueryAsync(request, query, processor).ConfigureAwait(false), _context);

            private static async Task<IQueryResult<TResponse>> ExecuteQueryAsync(TRequest request, IQuery<TRequest, TResponse> query, IMicroProcessor processor)
            {
                try
                {
                    return new QueryResult<TResponse>(await processor.ExecuteAsync(request ,query).ConfigureAwait(false));
                }
                catch (Exception exception)
                {
                    return new QueryResult<TResponse>(exception);
                }
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual async Task RunAsync<TRequest, TResponse>(IQueryTest<TRequest, TResponse> test)
        {
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ServiceProvider()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }
        
        private Task RunAsync<TRequest, TResponse>(IQueryTest<TRequest, TResponse> test, MicroProcessorTestContext context) =>
            new ExecuteQueryTestRunner<TRequest, TResponse>(this, test, context).RunAsync();

        #endregion

        #region [====== Services ======]

        private static readonly MicroProcessorConfiguration _MicroProcessor = new MicroProcessorConfiguration();

        /// <summary>
        /// Returns the processor-configuration for the <see cref="IMicroProcessor" /> that is to be used by all tests.
        /// </summary>
        protected static IMicroProcessorConfiguration MicroProcessor =>
            _MicroProcessor;        

        #endregion
    }
}

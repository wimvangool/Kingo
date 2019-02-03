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
        #region [====== TestEngine ======]

        private abstract class TestEngine<TMicroProcessorTest> where TMicroProcessorTest : IMicroProcessorTest
        {
            protected TestEngine(MicroProcessorTestRunner testRunner, TMicroProcessorTest test, MicroProcessorTestContext context)
            {
                TestRunner = testRunner;
                Test = test;
                Context = context;
                Result = new NullTestResult(test);
            }

            private MicroProcessorTestRunner TestRunner
            {
                get;
            }

            protected TMicroProcessorTest Test
            {
                get;
            }

            protected MicroProcessorTestContext Context
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

        #region [====== RunAsync - HandleMessageTest ======]

        private sealed class HandleMessageTestEngine<TMessage, TEventStream> : TestEngine<IMessageHandlerTest<TMessage, TEventStream>>, IMessageProcessor<TMessage> where TEventStream : EventStream
        {                      
            public HandleMessageTestEngine(MicroProcessorTestRunner testRunner, IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context) :
                base(testRunner, test, context) { }

            protected override Task WhenAsync() =>
                Test.WhenAsync(this, Context);

            public async Task HandleAsync(TMessage message, IMessageHandler<TMessage> handler = null)
            {
                var result = await HandleMessageAsync(message, handler, Context.Processor).ConfigureAwait(false);

                try
                {
                    Test.Then(message, result, Context);
                }
                finally
                {
                    Result = result;
                }                
            }

            private async Task<MessageHandlerResult<TEventStream>> HandleMessageAsync(TMessage message, IMessageHandler<TMessage> handler, IMicroProcessor processor)
            {
                try
                {
                    await processor.HandleAsync(message, handler).ConfigureAwait(false);                    
                }
                catch (Exception exception)
                {
                    return new MessageHandlerResult<TEventStream>(exception);
                }
                return new MessageHandlerResult<TEventStream>(Context.CommitEventStream(), stream =>
                {
                    Context.SetEventStream(Test, stream);
                });
            }
        }

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual async Task RunAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {            
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ResolveProcessor()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }        

        Task IMessageHandlerTestProcessor.HandleAsync<TMessage>(TMessage message, MicroProcessorTestContext context, IMessageHandler<TMessage> handler) =>
            RunAsync(new MessageHandlerTestStub<TMessage>(message, handler), context);

        Task IMessageHandlerTestProcessor.RunAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context) =>
            RunAsync(test, context);

        private Task RunAsync<TMessage, TEventStream>(IMessageHandlerTest<TMessage, TEventStream> test, MicroProcessorTestContext context) where TEventStream : EventStream =>
            new HandleMessageTestEngine<TMessage, TEventStream>(this, test, context).RunTestAsync();            

        #endregion

        #region [====== RunAsync - ExecuteQueryTest (1) ======]

        private sealed class ExecuteQueryTestEngine<TResponse> : TestEngine<IQueryTest<TResponse>>, IQueryProcessor<TResponse>
        {           
            public ExecuteQueryTestEngine(MicroProcessorTestRunner testRunner, IQueryTest<TResponse> test, MicroProcessorTestContext context) :
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

            private static async Task<QueryResult<TResponse>> ExecuteQueryAsync(IQuery<TResponse> query, IMicroProcessor processor)
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
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ResolveProcessor()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }
         
        private Task RunAsync<TResponse>(IQueryTest<TResponse> test, MicroProcessorTestContext context) =>
            new ExecuteQueryTestEngine<TResponse>(this, test, context).RunTestAsync();

        #endregion

        #region [====== RunAsync - ExecuteQueryTest (2) ======]

        private sealed class ExecuteQueryTestEngine<TRequest, TResponse> : TestEngine<IQueryTest<TRequest, TResponse>>, IQueryProcessor<TRequest, TResponse>
        {           
            public ExecuteQueryTestEngine(MicroProcessorTestRunner testRunner, IQueryTest<TRequest, TResponse> test, MicroProcessorTestContext context) :
                base(testRunner, test, context) { }

            protected override Task WhenAsync() =>
                Test.WhenAsync(this, Context);

            public async Task ExecuteAsync(TRequest request, IQuery<TRequest, TResponse> query)
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

            private static async Task<QueryResult<TResponse>> ExecuteQueryAsync(TRequest request, IQuery<TRequest, TResponse> query, IMicroProcessor processor)
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
            using (MicroProcessorTestContext.CreateScope(_MicroProcessor.ResolveProcessor()))
            {
                await RunAsync(test, MicroProcessorTestContext.Current).ConfigureAwait(false);
            }
        }
        
        private Task RunAsync<TRequest, TResponse>(IQueryTest<TRequest, TResponse> test, MicroProcessorTestContext context) =>
            new ExecuteQueryTestEngine<TRequest, TResponse>(this, test, context).RunTestAsync();

        #endregion

        #region [====== MicroProcessor ======]

        private static readonly MicroProcessorConfiguration _MicroProcessor = new MicroProcessorConfiguration();

        /// <summary>
        /// Returns the processor-configuration for the <see cref="IMicroProcessor" /> that is to be used by all tests.
        /// </summary>
        protected static IMicroProcessorConfiguration MicroProcessor =>
            _MicroProcessor;        

        #endregion
    }
}

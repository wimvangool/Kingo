using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test-classes that execute tests based on test's.
    /// </summary>
    public abstract class MicroProcessorTestRunner
    {
        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param>        
        protected virtual Task RunAsync<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test) where TEventStream : EventStream =>
            //new HandleMessageScenarioRunner(this, test);
            throw new NotImplementedException();

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param> 
        protected virtual Task RunAsync<TResponse>(IExecuteQueryTest<TResponse> test) =>
            throw new NotImplementedException();

        /// <summary>
        /// Runs the specified <paramref name="test" />.
        /// </summary>        
        /// <param name="test">The test to run.</param> 
        protected virtual Task RunAsync<TRequest, TResponse>(IExecuteQueryTest<TRequest, TResponse> test) =>
            throw new NotImplementedException();

        /// <summary>
        /// Creates and returns a new <see cref="IMicroProcessor"/> that is used to run all test's and that
        /// publishes all events to the specified <paramref name="serviceBus"/>.
        /// </summary>
        /// <param name="serviceBus">The service-bus to which all events are published.</param>
        /// <returns>A new <see cref="IMicroProcessor"/>.</returns>
        protected virtual IMicroProcessor CreateProcessor(IMicroServiceBus serviceBus) =>
            new MicroProcessor(serviceBus);        
    }
}

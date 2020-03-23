using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenReturningState<TRequest, TResponse> : MicroProcessorTestState, IWhenReturningState<TRequest, TResponse>
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenReturningState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Configuring a query of type '{typeof(IQuery<TRequest, TResponse>).FriendlyName()}'...";

        public IQueryTestRunner<TRequest, TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TRequest, TResponse> =>
            throw new NotImplementedException();
    }
}

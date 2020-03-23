using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenReturningState<TResponse> : MicroProcessorTestState, IWhenReturningState<TResponse>
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
            $"Configuring a query of type '{typeof(IQuery<TResponse>).FriendlyName()}'...";

        public IQueryTestRunner<TResponse> IsExecutedBy<TQuery>(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator) where TQuery : class, IQuery<TResponse> =>
            throw new NotImplementedException();
    }
}

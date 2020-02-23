using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class ReadyToRunTestState : MicroProcessorTestState, IReadyToRunTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly IEnumerable<MicroProcessorTestOperation> _givenOperations;

        protected ReadyToRunTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public Task ThenOutputIs<TException>(Action<TException, MicroProcessorTestContext> assertMethod = null) where TException : Exception =>
            // (await RunTestAsync(_test, _givenOperations).AssertOutputIs<TException>(assertMethod);
            throw new NotImplementedException();

        //protected abstract Task<object> RunTestAsync();
    }
}

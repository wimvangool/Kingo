using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class GivenStatementCollection
    {
        private readonly List<Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task>> _givenStatements;

        public GivenStatementCollection()
        {
            _givenStatements = new List<Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task>>();
        }

        public GivenStatementCollection Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            if (givenStatement == null)
            {
                throw new ArgumentNullException(nameof(givenStatement));
            }
            _givenStatements.Add(givenStatement);
            return this;
        }            

        public async Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

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

        public void Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement) =>
            _givenStatements.Add(givenStatement ?? throw new ArgumentNullException(nameof(givenStatement)));

        public async Task GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

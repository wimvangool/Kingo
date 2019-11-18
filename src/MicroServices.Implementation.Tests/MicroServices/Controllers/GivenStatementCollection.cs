using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class GivenStatementCollection
    {
        private readonly List<Func<IMessageHandlerOperationTestProcessor, MicroProcessorOperationTestContext, Task>> _givenStatements;

        public GivenStatementCollection()
        {
            _givenStatements = new List<Func<IMessageHandlerOperationTestProcessor, MicroProcessorOperationTestContext, Task>>();
        }

        public void Given(Func<IMessageHandlerOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement) =>
            _givenStatements.Add(givenStatement ?? throw new ArgumentNullException(nameof(givenStatement)));

        public async Task GivenAsync(IMessageHandlerOperationTestProcessor processor, MicroProcessorOperationTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

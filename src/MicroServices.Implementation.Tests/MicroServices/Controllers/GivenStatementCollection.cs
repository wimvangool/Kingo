using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class GivenStatementCollection
    {
        private readonly List<Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task>> _givenStatements;

        public GivenStatementCollection()
        {
            _givenStatements = new List<Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task>>();
        }

        public void Given(Func<IHandleMessageOperationTestProcessor, MicroProcessorOperationTestContext, Task> givenStatement) =>
            _givenStatements.Add(givenStatement ?? throw new ArgumentNullException(nameof(givenStatement)));

        public async Task GivenAsync(IHandleMessageOperationTestProcessor processor, MicroProcessorOperationTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

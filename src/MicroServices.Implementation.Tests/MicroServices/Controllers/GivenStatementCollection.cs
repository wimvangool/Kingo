using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class GivenStatementCollection
    {
        private readonly List<Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task>> _givenStatements;

        public GivenStatementCollection()
        {
            _givenStatements = new List<Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task>>();
        }

        public void Given(Func<IMicroProcessorOperationRunner, MicroProcessorOperationTestContext, Task> givenStatement) =>
            _givenStatements.Add(givenStatement ?? throw new ArgumentNullException(nameof(givenStatement)));

        public async Task GivenAsync(IMicroProcessorOperationRunner processor, MicroProcessorOperationTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

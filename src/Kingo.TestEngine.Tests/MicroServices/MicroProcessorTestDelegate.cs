using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal abstract class MicroProcessorTestDelegate : IMicroProcessorTest        
    {
        private readonly List<Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task>> _givenStatements;

        protected MicroProcessorTestDelegate()
        {
            _givenStatements = new List<Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task>>();
        }

        public MicroProcessorTestDelegate Given(Func<IMessageHandlerTestProcessor, MicroProcessorTestContext, Task> givenStatement)
        {
            if (givenStatement == null)
            {
                throw new ArgumentNullException(nameof(givenStatement));
            }
            _givenStatements.Add(givenStatement);
            return this;
        }            

        async Task IMicroProcessorTest.GivenAsync(IMessageHandlerTestProcessor processor, MicroProcessorTestContext context)
        {
            foreach (var givenStatement in _givenStatements)
            {
                await givenStatement.Invoke(processor, context);
            }
        }
    }
}

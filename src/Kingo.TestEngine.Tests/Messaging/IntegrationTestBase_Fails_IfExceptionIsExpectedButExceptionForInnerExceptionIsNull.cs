using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class IntegrationTestBase_Fails_IfExceptionIsExpectedButExceptionForInnerExceptionIsNull : IntegrationTestBaseTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
             throw new BadRequestException(new object(), _Message, new Exception(_Message));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public override async Task ThenAsync()
        {            
            try
            {
                await Result.IsExceptionOfTypeAsync<BadRequestException>(e =>
                {
                    AssertInnerExceptionIsOfType<Exception>(null, innerException => { });
                });
            }            
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}

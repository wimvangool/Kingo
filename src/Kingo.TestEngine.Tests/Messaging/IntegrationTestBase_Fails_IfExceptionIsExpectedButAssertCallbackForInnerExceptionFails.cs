using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class IntegrationTestBase_Fails_IfExceptionIsExpectedButAssertCallbackForInnerExceptionFails : IntegrationTestBaseTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context)
        {
            throw new BadRequestException(new object(), _Message, new Exception(_Message));
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {            
            var exceptionToThrow = new MetaAssertFailedException(_Message, null);

            try
            {
                await Result.IsExceptionOfTypeAsync<BadRequestException>(exception =>
                {
                    AssertInnerExceptionIsOfType<Exception>(exception, innerException =>
                    {
                        throw exceptionToThrow;
                    });
                });
            }        
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
            finally
            {
                await base.ThenAsync();
            }
        }
    }
}

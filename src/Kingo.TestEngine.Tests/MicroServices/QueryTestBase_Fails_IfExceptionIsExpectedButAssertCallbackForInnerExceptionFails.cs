using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryTestBase_Fails_IfExceptionIsExpectedButAssertCallbackForInnerExceptionFails : QueryTestBaseTest<object>
    {
        private const string _Message = "Test";

        protected override object ExecuteQuery(IMicroProcessorContext context) =>
            throw new BadRequestException(_Message, new Exception(_Message));

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public override async Task ThenAsync()
        {            
            var exceptionToThrow = new MetaAssertFailedException(_Message, null);

            try
            {
                await ThenResult().IsExceptionOfTypeAsync<BadRequestException>(exception =>
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

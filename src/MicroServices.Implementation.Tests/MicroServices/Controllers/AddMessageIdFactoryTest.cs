using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class AddMessageIdFactoryTest : MicroProcessorTest<MicroProcessor>
    {
        [TestMethod]
        public async Task AddMessageIdFactories_AddsDefaultFactory_IfNoFactoryTypesWereFound()
        {
            //Assert.AreEqual(0, ProcessorBuilder.Components.AddMessageIdFactories());

            //await CreateProcessor().ExecuteCommandAsync((message, context) =>
            //{
            //    Assert.IsNotNull(context.StackTrace.CurrentOperation.Message.MessageId);
            //    Assert.IsNull(context.StackTrace.CurrentOperation.Message.CorrelationId);
            //}, new object());
            throw new NotImplementedException();
        }
    }
}

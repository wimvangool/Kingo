using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public sealed class MessageHandlerFactoryForUnityTest : MessageHandlerFactoryTest
    {
        protected override MessageHandlerFactory CreateFactory()
        {
            return new MessageHandlerFactoryForUnity();
        }

        [TestMethod]
        public override void CreateInternalHandlersForObject_ReturnsExpectedHandler_IfOneHandlerForMessageExists()
        {
            base.CreateInternalHandlersForObject_ReturnsExpectedHandler_IfOneHandlerForMessageExists();
        }

        [TestMethod]
        public override void CreateExternalHandlersForObject_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            base.CreateExternalHandlersForObject_ReturnsNoHandlers_IfNoHandlersForMessageExist();
        }

        [TestMethod]
        public override void CreateInternalHandlersForCommand_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            base.CreateInternalHandlersForCommand_ReturnsNoHandlers_IfNoHandlersForMessageExist();
        }

        [TestMethod]
        public override void CreateExternalHandlersForCommand_ReturnsExpectedHandlers_IfTwoHandlersForMessageExist()
        {
            base.CreateExternalHandlersForCommand_ReturnsExpectedHandlers_IfTwoHandlersForMessageExist();
        }

        [TestMethod]
        public override void CreateInternalHandlersForDomainEvent_ReturnsExpectedHandlers_IfThreeHandlersForMessageExist()
        {
            base.CreateInternalHandlersForDomainEvent_ReturnsExpectedHandlers_IfThreeHandlersForMessageExist();
        }

        [TestMethod]
        public override void CreateExternalHandlersForDomainEvent_ReturnsNoHandlers_IfNoHandlersForMessageExist()
        {
            base.CreateExternalHandlersForDomainEvent_ReturnsNoHandlers_IfNoHandlersForMessageExist();
        }
    }
}

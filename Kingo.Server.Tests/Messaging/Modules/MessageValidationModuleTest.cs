using Kingo.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kingo.Messaging.Modules
{
    [TestClass]
    public sealed class MessageValidationModuleTest
    {
        private Mock<IMessageHandler<RequiredValueMessage<object>>> _nextHandlerMock;
        private RequiredValueMessage<object> _message;
        private IMessageHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _nextHandlerMock = new Mock<IMessageHandler<RequiredValueMessage<object>>>(MockBehavior.Strict); 
            _message = new RequiredValueMessage<object>();           
            _handler = new MessageHandlerWrapper<RequiredValueMessage<object>>(_message, _nextHandlerMock.Object);
        }        

        [TestMethod]
        public void Module_InvokesNextHandler_IfMessageIsValid()
        {
            _message.Value = new object();
            _nextHandlerMock.Setup(handler => handler.HandleAsync(_message)).Returns(AsyncMethod.Void);

            var module = new MessageValidationModule();
            {                         
                module.InvokeAsync(_handler).Wait(); 
            }
            _nextHandlerMock.VerifyAll();
        }

        [TestMethod]        
        public void Module_Throws_IfValidatorIsSpecifiedAndMessageIsInvalid()
        {
            _nextHandlerMock.Setup(handler => handler.HandleAsync(_message));

            var module = new MessageValidationModule();
            {
                module.InvokeAsync(_handler).WaitAndHandle<InvalidMessageException>();
            }            
        }        
    }
}

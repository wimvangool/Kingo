using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceComponents.Threading;

namespace ServiceComponents.ComponentModel.Server
{
    [TestClass]
    public sealed class MessageValidationModuleTest
    {
        private Mock<IMessageHandler<CommandStub>> _nextHandlerMock;        
        private CommandStub _command;
        private IMessageHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _nextHandlerMock = new Mock<IMessageHandler<CommandStub>>(MockBehavior.Strict);            
            _command = new CommandStub();
            _handler = new MessageHandlerWrapper<CommandStub>(_command, _nextHandlerMock.Object);
        }        

        [TestMethod]
        public void Module_InvokesNextHandler_IfMessageIsValid()
        {
            _command.Value = "Some Value";
            _nextHandlerMock.Setup(handler => handler.HandleAsync(_command)).Returns(AsyncMethod.Void);

            var module = new MessageValidationModule();
            {                         
                module.InvokeAsync(_handler).Wait(); 
            }
            _nextHandlerMock.VerifyAll();
        }

        [TestMethod]        
        public void Module_Throws_IfValidatorIsSpecifiedAndMessageIsInvalid()
        {
            _nextHandlerMock.Setup(handler => handler.HandleAsync(_command));

            var module = new MessageValidationModule();
            {
                module.InvokeAsync(_handler).WaitAndHandle<InvalidMessageException>();
            }            
        }        
    }
}

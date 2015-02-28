using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.ComponentModel.Server
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
            _nextHandlerMock.Setup(handler => handler.Handle(_command));

            using (var module = new MessageValidationModule())
            {                
                module.Invoke(_handler); 
            }
            _nextHandlerMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMessageException))]
        public void Module_Throws_IfValidatorIsSpecifiedAndMessageIsInvalid()
        {                      
            _nextHandlerMock.Setup(handler => handler.Handle(_command));

            using (var module = new MessageValidationModule())
            {
                module.Invoke(_handler);
            }
            _nextHandlerMock.VerifyAll();
        }        
    }
}

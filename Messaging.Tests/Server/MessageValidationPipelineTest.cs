using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.ComponentModel.Server
{
    [TestClass]
    public sealed class MessageValidationPipelineTest
    {
        private Mock<IMessageHandler<CommandStub>> _nextHandlerMock;        
        private CommandStub _command;

        [TestInitialize]
        public void Setup()
        {
            _nextHandlerMock = new Mock<IMessageHandler<CommandStub>>(MockBehavior.Strict);            
            _command = new CommandStub();
        }        

        [TestMethod]
        public void Module_InvokesNextHandler_IfMessageIsValid()
        {
            IMessageHandler<CommandStub> pipeline = new MessageValidationModule<CommandStub>(_nextHandlerMock.Object, null);

            _command.Value = "Some Value";
            _nextHandlerMock.Setup(handler => handler.Handle(_command));

            pipeline.Handle(_command);

            _nextHandlerMock.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidMessageException))]
        public void Module_Throws_IfValidatorIsSpecifiedAndMessageIsInvalid()
        {
            IMessageHandler<CommandStub> pipeline = new MessageValidationModule<CommandStub>(_nextHandlerMock.Object, null);
          
            _nextHandlerMock.Setup(handler => handler.Handle(_command));

            pipeline.Handle(_command);

            _nextHandlerMock.VerifyAll();
        }        
    }
}

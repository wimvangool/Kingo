using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class ConfigureMessagePipelineTest : MicroProcessorTest<MicroProcessor>
    {
        private sealed class SomeMessageHandler : IMessageHandler<SomeMessage>
        {
            public Task HandleAsync(SomeMessage message, MessageHandlerOperationContext context) =>
                Task.CompletedTask;
        }

        private sealed class SomeMessageSender : IMessageHandler<SomeMessage>
        {
            public Task HandleAsync(SomeMessage message, MessageHandlerOperationContext context)
            {
                context.MessageBus.Send(message);
                return Task.CompletedTask;
            }
        }

        private sealed class SomeMessagePublisher : IMessageHandler<SomeMessage>
        {
            public Task HandleAsync(SomeMessage message, MessageHandlerOperationContext context)
            {
                context.MessageBus.Publish(message);
                return Task.CompletedTask;
            }
        }

        private sealed class SomeQuery : IQuery<SomeMessage, SomeMessage>
        {
            public Task<SomeMessage> ExecuteAsync(SomeMessage message, QueryOperationContext context) =>
                Task.FromResult(message);
        }

        private sealed class SomeMessage
        {
            public static readonly SomeMessage ValidMessage = new SomeMessage(Guid.NewGuid().ToString());
            public static readonly SomeMessage InvalidMessage = new SomeMessage(null);

            private SomeMessage(string id)
            {
                Id = id;
            }

            [Required(AllowEmptyStrings = true)]
            public string Id
            {
                get;
            }
        }

        #region [====== Validation (Input - Commands) ======]

        [TestMethod]
        public async Task ExecuteCommand_AcceptsUndefinedCommand_IfNoValidationIsConfiguredForInputCommands()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessageHandler(), SomeMessage.ValidMessage);

            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestMessageException))]
        public async Task ExecuteCommand_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForInput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessageHandler(), SomeMessage.ValidMessage);
            }
            catch (BadRequestMessageException exception)
            {
                Assert.AreEqual(0, exception.ValidationErrors.Count);
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteCommand_AcceptsCommand_IfCommandIsNotValid_But_NoValidationIsConfiguredForInputCommands()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessageHandler(), SomeMessage.InvalidMessage);

            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestMessageException))]
        public async Task ExecuteCommand_Throws_IfCommandIsNotValid_And_ValidationIsConfiguredToValidateInputCommands()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.Commands);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessageHandler(), SomeMessage.InvalidMessage);
            }
            catch (BadRequestMessageException exception)
            {
                Assert.AreEqual(1, exception.ValidationErrors.Count);
                throw;
            }
        }

        #endregion

        #region [====== Validation (Input - Events) ======]

        [TestMethod]
        public async Task HandleEvent_AcceptsUndefinedEvent_IfNoValidationIsConfiguredForInputEvents()
        {
            var result = await CreateProcessor().HandleEventAsync(new SomeMessageHandler(), SomeMessage.ValidMessage);

            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleEvent_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForInput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().HandleEventAsync(new SomeMessageHandler(), SomeMessage.ValidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        [TestMethod]
        public async Task HandleEvent_AcceptsEvent_IfEventIsNotValid_But_NoValidationIsConfiguredForInputEvents()
        {
            var result = await CreateProcessor().HandleEventAsync(new SomeMessageHandler(), SomeMessage.InvalidMessage);

            Assert.AreEqual(0, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task HandleEvent_Throws_IfEventIsNotValid_And_ValidationIsConfiguredToValidateInputEvents()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.Events);
            });

            try
            {
                await CreateProcessor().HandleEventAsync(new SomeMessageHandler(), SomeMessage.InvalidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        #endregion

        #region [====== Validation (Input - Requests) ======]

        [TestMethod]
        public async Task ExecuteQuery_AcceptsUndefinedRequest_IfNoValidationIsConfiguredForInputRequests()
        {
            var result = await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.ValidMessage);

            Assert.IsNotNull(result.Output.Content.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestMessageException))]
        public async Task ExecuteQuery_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForInput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.ValidMessage);
            }
            catch (BadRequestMessageException exception)
            {
                Assert.AreEqual(0, exception.ValidationErrors.Count);
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteQuery_AcceptsRequest_IfRequestIsNotValid_But_NoValidationIsConfiguredForInputRequests()
        {
            var result = await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.InvalidMessage);

            Assert.IsNull(result.Output.Content.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(BadRequestMessageException))]
        public async Task ExecuteQuery_Throws_IfRequestIsNotValid_And_ValidationIsConfiguredToValidateInputRequests()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Input, MessageValidationOptions.Requests);
            });

            try
            {
                await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.InvalidMessage);
            }
            catch (BadRequestMessageException exception)
            {
                Assert.AreEqual(1, exception.ValidationErrors.Count);
                throw;
            }
        }

        #endregion

        #region [====== Validation (Output - Commands) ======]

        [TestMethod]
        public async Task Send_AcceptsUndefinedCommand_IfNoValidationIsConfiguredForOutputCommands()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessageSender(), SomeMessage.ValidMessage);

            Assert.AreEqual(1, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task Send_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForOutput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessageSender(), SomeMessage.ValidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        [TestMethod]
        public async Task Send_AcceptsCommand_IfCommandIsNotValid_But_NoValidationIsConfiguredForOutputCommands()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessageSender(), SomeMessage.InvalidMessage);

            Assert.AreEqual(1, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task Send_Throws_IfCommandIsNotValid_And_ValidationIsConfiguredToValidateOutputCommands()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.Commands);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessageSender(), SomeMessage.InvalidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        #endregion

        #region [====== Validation (Output - Events) ======]

        [TestMethod]
        public async Task Publish_AcceptsUndefinedEvent_IfNoValidationIsConfiguredForOutputEvents()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessagePublisher(), SomeMessage.ValidMessage);

            Assert.AreEqual(1, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task Publish_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForOutput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessagePublisher(), SomeMessage.ValidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        [TestMethod]
        public async Task Publish_AcceptsEvent_IfEventIsNotValid_But_NoValidationIsConfiguredForOutputEvents()
        {
            var result = await CreateProcessor().ExecuteCommandAsync(new SomeMessagePublisher(), SomeMessage.InvalidMessage);

            Assert.AreEqual(1, result.Output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task Publish_Throws_IfEventIsNotValid_And_ValidationIsConfiguredToValidateOutputEvents()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.Events);
            });

            try
            {
                await CreateProcessor().ExecuteCommandAsync(new SomeMessagePublisher(), SomeMessage.InvalidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        #endregion

        #region [====== Validation (Output - Responses) ======]

        [TestMethod]
        public async Task ExecuteQuery_AcceptsUndefinedResponse_IfNoValidationIsConfiguredForOutputResponses()
        {
            var result = await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.ValidMessage);

            Assert.IsNotNull(result.Output.Content.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQuery_Throws_IfMessageIsUndefined_And_ValidationIsConfiguredToBlockUndefinedMessagesForOutput()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.BlockUndefined);
            });

            try
            {
                await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.ValidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        [TestMethod]
        public async Task ExecuteQuery_AcceptsResponse_IfResponseIsNotValid_But_NoValidationIsConfiguredForOutputResponses()
        {
            var result = await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.InvalidMessage);

            Assert.IsNull(result.Output.Content.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteQuery_Throws_IfResponseIsNotValid_And_ValidationIsConfiguredToValidateOutputResponses()
        {
            Processor.ConfigureMessagePipeline(messages =>
            {
                messages.Validate(MessageDirection.Output, MessageValidationOptions.Responses);
            });

            try
            {
                await CreateProcessor().ExecuteQueryAsync(new SomeQuery(), SomeMessage.InvalidMessage);
            }
            catch (InternalServerErrorException exception)
            {
                Assert.IsInstanceOfType(exception.InnerException, typeof(MessageValidationFailedException));
                throw;
            }
        }

        #endregion
    }
}

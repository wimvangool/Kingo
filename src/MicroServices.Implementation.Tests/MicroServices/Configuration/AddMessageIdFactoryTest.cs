using System.Threading.Tasks;
using Kingo.MicroServices.Controllers;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Configuration
{
    [TestClass]
    public sealed class AddMessageIdFactoryTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== ======]

        private sealed class SomeCommand : Message
        {
            protected override string GenerateMessageId() =>
                GetType().FriendlyName();
        }

        #endregion

        #region [====== CustomMessageIdFactory ======]

        private sealed class CustomMessageIdFactory : IMessageIdFactory<int>, IMessageIdFactory<string>
        {
            public int GenerateCount
            {
                get;
                private set;
            }

            public string GenerateMessageIdFor(int message)
            {
                try
                {
                    return message.ToString();
                }
                finally
                {
                    GenerateCount++;
                }
            }

            public string GenerateMessageIdFor(string message)
            {
                try
                {
                    return message + message;
                }
                finally
                {
                    GenerateCount++;
                }
            }
        }

        #endregion

        [TestMethod]
        public async Task ProcessorBuilder_UsesDefaultMessageIdFactory_IfNoFactoriesWereAdded_And_MessageDoesNotImplementIMessageInterface()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
            {
                // The event will have it's own unique MessageId, and its CorrelationId will be set
                // to that of the Command's MessageId that triggered this event.
                Assert.IsNotNull(context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreNotEqual(message, context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreEqual(message, context.StackTrace.CurrentOperation.Message.CorrelationId);
            });

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The MessageIdFactory will generate a new MessageId for the command,
                // but its CorrelationId will be null, since its the root message with no message
                // to correlate it with.
                Assert.IsNotNull(context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.IsNull(context.StackTrace.CurrentOperation.Message.CorrelationId);

                context.MessageBus.PublishEvent(context.StackTrace.CurrentOperation.Message.MessageId);
            }, new object());
        }

        [TestMethod]
        public async Task ProcessorBuilder_UsesMessageSpecificImplementation_IfNoFactoriesWereAdded_And_MessageImplementsIMessageInterface()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
            {
                // The event will have it's own unique MessageId, and its CorrelationId will be set
                // to that of the Command's MessageId that triggered this event.
                Assert.IsNotNull(context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreNotEqual(message, context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreEqual(message, context.StackTrace.CurrentOperation.Message.CorrelationId);
            });

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The message will generate its own message-id.
                Assert.AreEqual("SomeCommand",context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.IsNull(context.StackTrace.CurrentOperation.Message.CorrelationId);

                context.MessageBus.PublishEvent(context.StackTrace.CurrentOperation.Message.MessageId);
            }, new SomeCommand());
        }

        [TestMethod]
        public async Task ProcessorBuilder_UsesDefaultMessageIdFactory_IfCustomFactoryWasAdded_But_CustomFactoryDoesNotMatchMessageType()
        {
            var factory = new CustomMessageIdFactory();

            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.AddInstance<int>(factory.GenerateMessageIdFor));

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The MessageIdFactory will generate a new MessageId for the command,
                // but will not use the specified factory for that, since the message-types don't match.
                Assert.AreEqual(36, context.StackTrace.CurrentOperation.Message.MessageId.Length);
            }, new object());

            Assert.AreEqual(0, factory.GenerateCount);
        }

        [TestMethod]
        public async Task ProcessorBuilder_UsesCustomMessageIdFactoryInstances_IfCustomFactoriesWereAdded_And_CustomFactoriesMatchMessageTypes()
        {
            var factory = new CustomMessageIdFactory();

            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.AddInstance<int>(factory.GenerateMessageIdFor));
            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.AddInstance<string>(factory.GenerateMessageIdFor));

            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
            {
                // The event will have it's own unique MessageId, and its CorrelationId will be set
                // to that of the Command's MessageId that triggered this event.
                Assert.AreEqual("1010", context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.CorrelationId);
                Assert.AreEqual(message, context.StackTrace.CurrentOperation.Message.CorrelationId);
            });

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The MessageIdFactory will generate a new MessageId for the command,
                // using the specified factory for that, since the message-types match.
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.MessageId);

                context.MessageBus.PublishEvent(context.StackTrace.CurrentOperation.Message.MessageId);
            }, 10);

            Assert.AreEqual(2, factory.GenerateCount);
        }

        [TestMethod]
        public async Task ProcessorBuilder_UsesCustomMessageIdFactoryType_IfCustomFactoryWasAdded_And_CustomFactoryMatchesMessageTypes()
        {
            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.Add<CustomMessageIdFactory>());

            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
            {
                // The event will have it's own unique MessageId, and its CorrelationId will be set
                // to that of the Command's MessageId that triggered this event.
                Assert.AreEqual("1010", context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.CorrelationId);
                Assert.AreEqual(message, context.StackTrace.CurrentOperation.Message.CorrelationId);
            });

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The MessageIdFactory will generate a new MessageId for the command,
                // using the specified factory for that, since the message-types match.
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.MessageId);

                context.MessageBus.PublishEvent(context.StackTrace.CurrentOperation.Message.MessageId);
            }, 10);
        }

        [TestMethod]
        public async Task ProcessorBuilder_UsesCustomMessageIdFactoryInstance_IfCustomFactoryWasAdded_And_CustomFactoryMatchesMessageTypes()
        {
            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.Add<CustomMessageIdFactory>());

            ProcessorBuilder.MessageHandlers.AddInstance<string>((message, context) =>
            {
                // The event will have it's own unique MessageId, and its CorrelationId will be set
                // to that of the Command's MessageId that triggered this event.
                Assert.AreEqual("1010", context.StackTrace.CurrentOperation.Message.MessageId);
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.CorrelationId);
                Assert.AreEqual(message, context.StackTrace.CurrentOperation.Message.CorrelationId);
            });

            await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                // The MessageIdFactory will generate a new MessageId for the command,
                // using the specified factory for that, since the message-types match.
                Assert.AreEqual("10", context.StackTrace.CurrentOperation.Message.MessageId);

                context.MessageBus.PublishEvent(context.StackTrace.CurrentOperation.Message.MessageId);
            }, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(InternalServerErrorException))]
        public async Task ExecuteCommand_Throws_IfMultipleFactoriesWereRegisteredForSameMessageType()
        {
            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.AddInstance<object>(message => message.ToString()));
            Assert.IsTrue(ProcessorBuilder.MessageIdFactories.AddInstance<object>(message => message.ToString()));

            try
            {
                await CreateProcessor().ExecuteCommandAsync((message, context) => { }, new object());
            }
            catch (InternalServerErrorException exception)
            {
                Assert.AreEqual("Cannot generate message-id for message of type 'Object' because multiple factories were configured for this message-type: MessageIdFactoryInstance<Object>, MessageIdFactoryInstance<Object>.", exception.Message);
                throw;
            }
        }
    }
}

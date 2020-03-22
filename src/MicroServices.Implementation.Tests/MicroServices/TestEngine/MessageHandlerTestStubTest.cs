using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.TestEngine
{
    [TestClass]
    public sealed class MessageHandlerTestStubTest : MicroProcessorTestStubTest<MessageHandlerTestStub>
    {
        #region [====== CreateMicroProcessorTest ======]

        protected override MessageHandlerTestStub CreateMicroProcessorTest() =>
            new MessageHandlerTestStub();

        #endregion

        #region [====== When ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfSetupWasNotCalled()
        {
            await RunTestAsync(test => test.When<object>(), false);
        }

        [TestMethod]
        public async Task When_ReturnsWhenCommandOrEventState_IfTestEngineIsInReadyToConfigureState_And_NoGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>();

                Assert.IsNotNull(state);
                Assert.AreEqual("Configuring a message handler to process message of type 'Object'...", state.ToString());
            });
        }

        [TestMethod]
        public async Task When_ReturnsWhenCommandOrEventState_IfTestEngineIsInReadyToConfigureState_And_SomeGivenOperationsWereScheduled()
        {
            await RunTestAsync(test =>
            {
                test.Given().TimeIs(2020, 2, 5);
                test.Given<object>().IsHandledBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(test.When<object>());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_Throws_IfTestEngineIsInWhenCommandOrEventState()
        {
            await RunTestAsync(test =>
            {
                test.When<object>();
                test.When<object>();
            });
        }

        #endregion

        #region [====== When<...>().IsExecutedBy<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedByCommandHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsExecutedByCommandHandler(null, new NullHandler());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedByCommandHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsExecutedByCommandHandler((operation, context) => { }, null);
            });
        }

        [TestMethod]
        public async Task WhenIsExecutedByCommandHandler_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenCommandOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>().IsExecutedByCommandHandler((operation, context) => { }, new NullHandler());

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsExecutedBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsExecutedBy<NullHandler>(null);
            });
        }

        [TestMethod]
        public async Task WhenIsExecutedBy_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenCommandOrEventState()
        {
            await RunTestAsync(test =>
            {
                var state = test.When<object>().IsExecutedBy<NullHandler>((operation, context) => { });

                Assert.IsNotNull(state);
                Assert.AreEqual("Ready to process message of type 'Object' with message handler of type 'NullHandler'...", state.ToString());
            });
        }

        #endregion

        #region [====== When<...>().IsHandledBy<...>() ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledByEventHandler_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsHandledByEventHandler(null, new NullHandler());
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledByEventHandler_Throws_IfMessageHandlerIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsHandledByEventHandler((operation, context) => { }, null);
            });
        }

        [TestMethod]
        public async Task WhenIsHandledByEventHandler_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenEventOrEventState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.When<object>().IsHandledByEventHandler((operation, context) => { }, new NullHandler()));
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WhenIsHandledBy_Throws_IfConfiguratorIsNull()
        {
            await RunTestAsync(test =>
            {
                test.When<object>().IsHandledBy<NullHandler>(null);
            });
        }

        [TestMethod]
        public async Task WhenIsHandledBy_ReturnsReadyToRunMessageHandlerTestState_IfTestEngineIsInWhenEventOrEventState()
        {
            await RunTestAsync(test =>
            {
                Assert.IsNotNull(test.When<object>().IsHandledBy<NullHandler>((operation, context) => { }));
            });
        }

        #endregion

        #region [====== ThenOutputIsException(...) ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfGivenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                var errorMessage = Guid.NewGuid().ToString();

                test.Given<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);
                });

                try
                {
                    await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                    {
                        operation.Message = new object();
                    }, (message, context) =>
                    {
                        throw context.NewBadRequestException();
                    }).ThenOutputIs<BadRequestException>();
                }
                catch (TestFailedException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(InternalServerErrorException));
                    Assert.AreEqual(errorMessage, exception.InnerException.Message);
                    throw;
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                {
                    operation.Message = new object();
                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfWhenOperationThrowsExceptionOfDifferentType()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewInternalServerErrorException();
                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfExpectedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewBadRequestException();
                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfWhenOperationThrowsExceptionOfDerivedType_And_NoAssertMethodIsSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewNotFoundException();
                }).ThenOutputIs<BadRequestException>();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsException_Throws_IfSpecifiedAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewBadRequestException();
                }).ThenOutputIs<BadRequestException>((message, exception, context) =>
                {
                    throw NewRandomException();
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfSpecifiedAssertMethodDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = inputMessage;
                }, (message, context) =>
                {
                    throw context.NewBadRequestException();
                }).ThenOutputIs<BadRequestException>((message, exception, context) =>
                {
                    Assert.AreSame(inputMessage, message);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsException_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewBadRequestException();
                }).ThenOutputIs<BadRequestException>();
            }, true, true);
        }

        #endregion

        #region [====== ThenOutputIsMessageStream ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsMessageStream_Throws_IfGivenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                var errorMessage = Guid.NewGuid().ToString();

                test.Given<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw context.NewInternalServerErrorException(errorMessage);
                });

                try
                {
                    await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                    {
                        operation.Message = new object();
                    }).ThenOutputIsMessageStream();
                }
                catch (TestFailedException exception)
                {
                    Assert.IsInstanceOfType(exception.InnerException, typeof(InternalServerErrorException));
                    Assert.AreEqual(errorMessage, exception.InnerException.Message);
                    throw;
                }
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsMessageStream_Throws_IfWhenOperationThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = new object();
                }, (message, context) =>
                {
                    throw NewRandomException();
                }).ThenOutputIsMessageStream();
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfWhenOperationDoesNotThrowException_And_AssertMethodIsNotSpecified()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                {
                    operation.Message = new object();
                }).ThenOutputIsMessageStream();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsMessageStream_Throws_IfAssertMethodThrowsException()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                {
                    operation.Message = new object();
                }).ThenOutputIsMessageStream((message, stream, context) =>
                {
                    throw NewRandomException();
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfAssertMethodDoesNotThrowException()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = inputMessage;
                }, (message, context) =>
                {
                    context.MessageBus.PublishEvent(new object());
                }).ThenOutputIsMessageStream((message, stream, context) =>
                {
                    Assert.AreSame(inputMessage, message);
                    Assert.AreEqual(1, stream.Count);
                });
            });
        }

        [TestMethod]
        public async Task ThenOutputIsMessageStream_Succeeds_IfVerificationSucceeds_And_TearDownIsExecutedAfterwards()
        {
            await RunTestAsync(async test =>
            {
                await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                {
                    operation.Message = new object();
                }).ThenOutputIsMessageStream();
            }, true, true);
        }

        #endregion

        #region [====== ThenOutputIsEmptyStream ======]

        [TestMethod]
        public async Task ThenOutputIsEmptyStream_Succeeds_IfOperationProducesEmptyStream()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When<object>().IsExecutedBy<NullHandler>((operation, context) =>
                {
                    operation.Message = inputMessage;
                }).ThenOutputIsEmptyStream();
            });
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task ThenOutputIsEmptyStream_Throws_IfOperationProducesEmptyStream()
        {
            await RunTestAsync(async test =>
            {
                var inputMessage = new object();

                await test.When<object>().IsExecutedByCommandHandler((operation, context) =>
                {
                    operation.Message = inputMessage;
                }, (message, context) =>
                {
                    context.MessageBus.PublishEvent(new object());
                }).ThenOutputIsEmptyStream();
            });
        }

        #endregion
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorTestRunnerTest : MicroProcessorTestRunner
    {
        #region [====== MessageHandlerTests (1) ======]

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesUntypedEventStream_And_ResultIsNotProduced()
        {
            var test = CreateMessageHandlerTest()
                .When((messageProcessor, testContext) => Task.CompletedTask)
                .ThenResultIsEventStream();

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesUntypedEventStream_And_ResultIsNotVerified()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (message, processor) => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfEmptyEventStreamIsExpected_But_ResultIsException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .ThenResultIsEventStream();

            await RunAsync(test);
        }        

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfEmptyEventStreamIsExpected_And_NoEventsWerePublished()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) => { });
                })
                .ThenResultIsEventStream();

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfEmptyEventStreamIsExpected_But_OneEventWasPublished()
        {
            var @event = new object();
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEmptyStream();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfOneEventIsExpected_But_NoEventsWerePublished()
        {            
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) => { });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream(stream =>
                    {
                        Assert.AreEqual(1, stream.Count);
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfOneEventIsExpected_And_OneEventWasPublished()
        {
            var @event = new object();
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream(stream =>
                    {
                        Assert.AreSame(@event, stream[0]);                        
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfOneEventIsExpected_But_DifferentEventWasPublished()
        {
            var @event = new object();
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream(stream =>
                    {                        
                        Assert.AreNotSame(@event, stream[0]);                     
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfExceptionIsExpected_But_NoExceptionWasThrown()
        {            
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) => { });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificExceptionIsExpected_But_DifferentTypeOfExceptionWasThrown()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<BusinessRuleException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificExceptionIsExpected_And_ThatTypeOfExceptionWasThrown()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificExceptionIsExpected_And_DerivedTypeOfExceptionWasThrown()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<MicroProcessorException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificExceptionIsExpected_But_AssertionOfExceptionFails()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception =>
                    {
                        Assert.AreEqual(Guid.NewGuid().ToString("N"), exception.Message);
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificExceptionIsExpected_And_AssertionOfExceptionSucceeds()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_ExceptionHasNoInnerException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomInternalServerError();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_ExceptionHasDifferentTypeOfInnerException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<InvalidOperationException>();
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificInnerExceptionIsExpected_And_ExceptionHasThatTypeOfInnerException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<BusinessRuleException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificInnerExceptionIsExpected_And_ExceptionHasDerivedTypeOfInnerException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<MessageHandlerException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_AssertionOfInnerExceptionFails()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<BusinessRuleException>(exception =>
                        {
                            Assert.AreEqual(Guid.NewGuid().ToString(), exception.Message);
                        });
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificInnerExceptionIsExpected_And_AssertionOfInnerExceptionSucceeds()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<BusinessRuleException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfWhenStatementAttemptsToObtainEventStreamThatIsNotFound()
        {
            var testA = CreateMessageHandlerTest();

            var testB = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    try
                    {
                        testContext.GetEventStream(testA);
                    }
                    finally
                    {
                        await messageProcessor.HandleAsync(new object(), (processor, context) => { });
                    }                    
                })
                .ThenResultIsEventStream();
            
            await RunAsync(testB);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_CorrectlyResolvesScopedDependencies_IfDependencyIsNeededByMultipleHandlers()
        {
            var testA = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        Assert.IsNotNull(MicroProcessorTestContext.Current);

                        context.ServiceProvider.GetRequiredService<InvocationCounter>().Increment();
                    });
                })
                .ThenResultIsEventStream();

            var testB = CreateMessageHandlerTest()
                .Given(async (processor, testContext) =>
                {
                    await processor.RunAsync(testA, testContext);
                })
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (message, context) =>
                    {
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().AssertExactly(1);
                    });
                })
                .ThenResultIsEventStream();

            await RunAsync(testB);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfGivenRunsAnotherTest_And_WhenStatementAttemptsToObtainEventStreamThatIsFound()
        {
            var @event = new object();

            var testA = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream();
                });

            var testB = CreateMessageHandlerTest()
                .Given(async (processor, testContext) =>
                {
                    await processor.RunAsync(testA, testContext);
                })
                .When(async (messageProcessor, testContext) =>
                {
                    testContext.GetEventStream(testA).AssertEvent<object>(0, actualEvent =>
                    {
                        Assert.AreSame(@event, actualEvent);
                    });

                    await messageProcessor.HandleAsync(new object(), (message, context) => { });
                })
                .ThenResultIsEventStream();

            await RunAsync(testB);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_Succeeds_IfGivenHandlesAnotherMessage_And_WhenStatementAttemptsToObtainEventStreamThatIsFound()
        {            
            var test = CreateMessageHandlerTest()
                .Given(async (testProcessor, testContext) =>
                {
                    await testProcessor.HandleAsync(new object(), testContext, (processor, context) =>
                    {
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().Increment();
                    });
                })
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (message, context) =>
                    {
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().AssertExactly(1);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream();
                });

            await RunAsync(test);
        }        

        private static Exception NewRandomBusinessRuleException() =>
            new BusinessRuleException(Guid.NewGuid().ToString("N"));

        private static Exception NewRandomInternalServerError() =>
            new InternalServerErrorException(Guid.NewGuid().ToString("N"));

        private static MessageHandlerTestDelegate CreateMessageHandlerTest() =>
            new MessageHandlerTestDelegate();

        #endregion

        #region [====== MessageHandlerTests (2) ======]

        private sealed class OutputStream : EventStream
        {
            public OutputStream(EventStream stream) :
                base(stream) { }

            public int Value =>
                GetEvent<int>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesTypedEventStream_And_ResultIsNotProduced()
        {
            var test = CreateMessageHandlerTest<OutputStream>()
                .When((messageProcessor, testContext) => Task.CompletedTask)
                .Then((message, result, testContext) => result.IsEventStream(stream => new OutputStream(stream)));

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(MicroProcessorTestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesTypedEventStream_And_ResultIsNotVerified()
        {
            var test = CreateMessageHandlerTest<OutputStream>()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (message, processor) => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_Succeeds_IfTypedEventStreamIsCorrectlyStoredAndRetrieved()
        {
            var testA = CreateMessageHandlerTest<OutputStream>()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(new object(), (message, context) =>
                    {
                        context.EventBus.Publish(DateTimeOffset.UtcNow.Millisecond);
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream(stream => new OutputStream(stream));
                });

            var testB = CreateMessageHandlerTest()
                .Given(async (testProcessor, testContext) =>
                {
                    await testProcessor.RunAsync(testA, testContext);
                })
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleAsync(testContext.GetEventStream(testA).Value, (message, context) =>
                    {
                        Assert.IsInstanceOfType(message, typeof(int));
                    });
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEmptyStream();
                });

            await RunAsync(testB);
        }

        private static MessageHandlerTestDelegate<TEventStream> CreateMessageHandlerTest<TEventStream>() where TEventStream : EventStream =>
            new MessageHandlerTestDelegate<TEventStream>();

        #endregion

        #region [====== QueryTests (1) ======]

        #endregion

        #region [====== QueryTests (2) ======]

        #endregion

        #region [====== SetupAssembly ======]

        [AssemblyInitialize]
        public static void SetupAssembly(TestContext context)
        {
            MicroProcessor.Setup().Configure(services =>
            {
                services.AddScoped<InvocationCounter>();
            });                
        }

        #endregion
    }
}

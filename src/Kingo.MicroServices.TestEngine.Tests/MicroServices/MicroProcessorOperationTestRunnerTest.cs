using System;
using System.Threading.Tasks;
using Kingo.MicroServices.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{    
    [TestClass]
    public sealed class MicroProcessorOperationTestRunnerTest : MicroProcessorOperationTestRunner
    {
        #region [====== MessageHandlerTests (1) ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesUntypedEventStream_And_GivenThrowsException()
        {
            var test = CreateMessageHandlerTest()
                .Given((processor, context) => throw NewRandomBusinessRuleException());                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesUntypedEventStream_And_ResultIsNotProduced()
        {
            var test = CreateMessageHandlerTest()
                .When((messageProcessor, testContext) => Task.CompletedTask);                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesUntypedEventStream_And_ResultIsNotVerified()
        {
            var test = CreateMessageHandlerTest()
                .Then((message, result, testContext) => { });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfEmptyEventStreamIsExpected_But_ResultIsException()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
                });                

            await RunAsync(test);
        }        

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfEmptyEventStreamIsExpected_And_NoEventsWerePublished()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) => { }, new object());
                });                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfEmptyEventStreamIsExpected_But_OneEventWasPublished()
        {
            var @event = new object();
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEmptyStream();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfOneEventIsExpected_But_NoEventsWerePublished()
        {            
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) => { }, new object());
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
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    }, new object());
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
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfOneEventIsExpected_But_DifferentEventWasPublished()
        {
            var @event = new object();
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    }, new object());
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
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfExceptionIsExpected_But_NoExceptionWasThrown()
        {            
            var test = CreateMessageHandlerTest()                
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificExceptionIsExpected_But_DifferentTypeOfExceptionWasThrown()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
                    await messageProcessor.HandleEventAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<MicroProcessorOperationException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificExceptionIsExpected_But_AssertionOfExceptionFails()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception =>
                    {
                        Assert.AreEqual(RandomMessage(), exception.Message);
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
                    await messageProcessor.HandleEventAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception => { });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsNull()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomInternalServerError();
                    }, new object());
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
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsOfOtherType()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfThatType()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleEventAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
        public async Task RunMessageHandlerTest_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfDerivedType()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.HandleEventAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<MessageHandlerOperationException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfSpecificInnerExceptionIsExpected_But_AssertionOfInnerExceptionFails()
        {
            var test = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
                    await messageProcessor.HandleEventAsync((processor, context) =>
                    {
                        throw NewRandomBusinessRuleException();
                    }, new object());
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
        [ExpectedException(typeof(TestFailedException))]
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
                        await messageProcessor.ExecuteCommandAsync((processor, context) => { }, new object());
                    }
                });                
            
            await RunAsync(testB);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_CorrectlyResolvesScopedDependencies_IfDependencyIsNeededByMultipleHandlers()
        {
            var testA = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {                        
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().Increment();
                    }, new object());
                });

            var testB = CreateMessageHandlerTest()
                .Given(async (processor, testContext) =>
                {
                    await processor.RunAsync(testA, testContext);
                })
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((message, context) =>
                    {
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().AssertExactly(1);
                    }, new object());
                });                

            await RunAsync(testB);
        }

        [TestMethod]        
        public async Task RunMessageHandlerTest_Succeeds_IfGivenRunsAnotherTest_And_WhenStatementAttemptsToObtainEventStreamThatIsFound()
        {
            var @event = new object();

            var testA = CreateMessageHandlerTest()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((processor, context) =>
                    {
                        context.EventBus.Publish(@event);
                    }, new object());
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

                    await messageProcessor.ExecuteCommandAsync((message, context) => { }, new object());
                });                

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
                    await messageProcessor.ExecuteCommandAsync((message, context) =>
                    {
                        context.ServiceProvider.GetRequiredService<InvocationCounter>().AssertExactly(1);
                    }, new object());
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEventStream();
                });

            await RunAsync(test);
        }

        private static HandleMessageTestDelegate CreateMessageHandlerTest() =>
            new HandleMessageTestDelegate();

        #endregion

        #region [====== MessageHandlerTests (2) ======]

        private sealed class OutputStream : EventStream
        {            
            public OutputStream() :
                this(Empty) { }

            public OutputStream(EventStream stream) :
                base(stream) { }

            public int Value =>
                GetEvent<int>(0);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesTypedEventStream_And_GivenThrowsException()
        {
            var test = CreateMessageHandlerTest<OutputStream>()
                .Given((processor, context) => throw NewRandomBusinessRuleException());                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesTypedEventStream_And_ResultIsNotProduced()
        {
            var test = CreateMessageHandlerTest<OutputStream>()
                .When((messageProcessor, testContext) => Task.CompletedTask);                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunMessageHandlerTest_Throws_IfTestUsesTypedEventStream_And_ResultIsNotVerified()
        {
            var test = CreateMessageHandlerTest<OutputStream>()
                .Then((message, result, testContext) => { });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunMessageHandlerTest_Succeeds_IfTypedEventStreamIsCorrectlyStoredAndRetrieved()
        {
            var testA = CreateMessageHandlerTest<OutputStream>()
                .When(async (messageProcessor, testContext) =>
                {
                    await messageProcessor.ExecuteCommandAsync((message, context) =>
                    {
                        context.EventBus.Publish(DateTimeOffset.UtcNow.Millisecond);
                    }, new object());
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
                    await messageProcessor.ExecuteCommandAsync((message, context) =>
                    {
                        Assert.IsInstanceOfType(message, typeof(int));
                    }, testContext.GetEventStream(testA).Value);
                })
                .Then((message, result, testContext) =>
                {
                    result.IsEmptyStream();
                });

            await RunAsync(testB);
        }

        private static HandleMessageTestDelegate<TEventStream> CreateMessageHandlerTest<TEventStream>() where TEventStream : EventStream, new() =>
            new HandleMessageTestDelegate<TEventStream>();

        #endregion

        #region [====== QueryTests (1) ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfGivenThrowsException()
        {
            var test = CreateQueryTest()
                .Given((processor, context) => throw NewRandomBusinessRuleException());                                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfResultIsNotProduced()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => Task.CompletedTask);                

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfResultIsNotVerified()
        {
            var test = CreateQueryTest()
                .Then((result, testContext) => { });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfResponseExpected_But_ResultIsException()
        {
            var test = CreateQueryTest()
                .When(async (queryProcessor, testContext) =>
                {
                    await queryProcessor.ExecuteAsync(context =>
                    {
                        throw NewRandomInternalServerError();
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfSpecificResponseExpected_But_OtherResponseIsReturned()
        {
            var test = CreateQueryTest()
                .Then((result, testContext) =>
                {
                    result.IsResponse(Assert.IsNotNull);
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunQueryTest1_Succeeds_IfSpecificResponseExpected_And_ThatResponseIsReturned()
        {
            var expectedResponse = new object();
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context => Task.FromResult(expectedResponse)))
                .Then((result, testContext) =>
                {
                    result.IsResponse(response => Assert.AreSame(expectedResponse, response));
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfSpecificExceptionIsExpected_But_NoExceptionWasThrown()
        {            
            var test = CreateQueryTest()                  
                .Then((result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Throws_IfSpecificExceptionIsExpected_But_OtherExceptionWasThrown()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((result, testContext) =>
                {
                    result.IsExceptionOfType<ArgumentNullException>();
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunQueryTest1_Succeeds_IfSpecificExceptionIsExpected_And_ThatExceptionWasThrown()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Fails_IfSpecificExceptionIsExpected_But_AssertionOfExceptionFailed()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception =>
                    {
                        Assert.AreEqual(Guid.NewGuid().ToString(), exception.Message);
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest1_Succeeds_IfSpecificExceptionIsExpected_And_DerivedExceptionWasThrown()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Fails_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsNull()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Fails_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsOfOtherType()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<ArgumentNullException>();
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunQueryTest1_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfThatType()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<InvalidOperationException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest1_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfDerivedType()
        {
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest1_Fails_IfSpecificInnerExceptionIsExpected_And_AssertionOfInnerExceptionFails()
        {
            var exception = NewRandomTechnicalException();
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw exception;
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>(innerException =>
                        {
                            Assert.AreNotSame(exception, innerException);
                        });
                });

            await RunAsync(test);
        }

        [TestMethod]        
        public async Task RunQueryTest1_Succeeds_IfSpecificInnerExceptionIsExpected_And_AssertionOfInnerExceptionSucceeds()
        {
            var exception = NewRandomTechnicalException();
            var test = CreateQueryTest()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(context =>
                {
                    throw exception;
                }))
                .Then((result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>(innerException =>
                        {
                            Assert.AreSame(exception, innerException);
                        });
                });

            await RunAsync(test);
        }

        private static ExecuteQueryTestDelegate<object> CreateQueryTest() =>
            new ExecuteQueryTestDelegate<object>();

        #endregion

        #region [====== QueryTests (2) ======]

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfGivenThrowsException()
        {
            var test = CreateQueryTest<object>()
                .Given((processor, context) => throw NewRandomBusinessRuleException());

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfResultIsNotProduced()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => Task.CompletedTask);

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfResultIsNotVerified()
        {
            var test = CreateQueryTest<object>()
                .Then((request, result, testContext) => { });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfResponseExpected_But_ResultIsException()
        {
            var test = CreateQueryTest<object>()
                .When(async (queryProcessor, testContext) =>
                {
                    await queryProcessor.ExecuteAsync(new object(), (message, context) =>
                    {
                        throw NewRandomInternalServerError();
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfSpecificResponseExpected_But_OtherResponseIsReturned()
        {
            var test = CreateQueryTest<object>()
                .Then((request, result, testContext) =>
                {
                    result.IsResponse(Assert.IsNotNull);
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificResponseExpected_And_ThatResponseIsReturned()
        {
            var expectedResponse = new object();
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) => Task.FromResult(expectedResponse)))
                .Then((request, result, testContext) =>
                {
                    result.IsResponse(response => Assert.AreSame(expectedResponse, response));
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfSpecificExceptionIsExpected_But_NoExceptionWasThrown()
        {
            var test = CreateQueryTest<object>()
                .Then((request, result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Throws_IfSpecificExceptionIsExpected_But_OtherExceptionWasThrown()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((request, result, testContext) =>
                {
                    result.IsExceptionOfType<ArgumentNullException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificExceptionIsExpected_And_ThatExceptionWasThrown()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((request, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Fails_IfSpecificExceptionIsExpected_But_AssertionOfExceptionFailed()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((request, result, testContext) =>
                {
                    result.IsExceptionOfType<InternalServerErrorException>(exception =>
                    {
                        Assert.AreEqual(Guid.NewGuid().ToString(), exception.Message);
                    });
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificExceptionIsExpected_And_DerivedExceptionWasThrown()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((request, result, testContext) =>
                {
                    result.IsExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Fails_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsNull()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomInternalServerError();
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Fails_IfSpecificInnerExceptionIsExpected_But_InnerExceptionIsOfOtherType()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<ArgumentNullException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfThatType()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<InvalidOperationException>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificInnerExceptionIsExpected_And_InnerExceptionIsOfDerivedType()
        {
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw NewRandomTechnicalException();
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>();
                });

            await RunAsync(test);
        }

        [TestMethod]
        [ExpectedException(typeof(TestFailedException))]
        public async Task RunQueryTest2_Fails_IfSpecificInnerExceptionIsExpected_And_AssertionOfInnerExceptionFails()
        {
            var exception = NewRandomTechnicalException();
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw exception;
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>(innerException =>
                        {
                            Assert.AreNotSame(exception, innerException);
                        });
                });

            await RunAsync(test);
        }

        [TestMethod]
        public async Task RunQueryTest2_Succeeds_IfSpecificInnerExceptionIsExpected_And_AssertionOfInnerExceptionSucceeds()
        {
            var exception = NewRandomTechnicalException();
            var test = CreateQueryTest<object>()
                .When((queryProcessor, testContext) => queryProcessor.ExecuteAsync(new object(), (message, context) =>
                {
                    throw exception;
                }))
                .Then((request, result, testContext) =>
                {
                    result
                        .IsExceptionOfType<InternalServerErrorException>()
                        .WithInnerExceptionOfType<Exception>(innerException =>
                        {
                            Assert.AreSame(exception, innerException);
                        });
                });

            await RunAsync(test);
        }

        private static ExecuteQueryTestDelegate<TRequest, object> CreateQueryTest<TRequest>() where TRequest : new() =>
            new ExecuteQueryTestDelegate<TRequest, object>();

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewRandomBusinessRuleException() =>
            new BusinessRuleException(RandomMessage());

        private static Exception NewRandomInternalServerError() =>
            new InternalServerErrorException(RandomMessage());

        private static Exception NewRandomTechnicalException() =>
            new InvalidOperationException(RandomMessage());

        private static string RandomMessage() =>
            Guid.NewGuid().ToString("N");

        #endregion

        #region [====== ConfigureServices ======]

        protected override IServiceCollection ConfigureServices(IServiceCollection services) =>
            services.AddMicroProcessor().AddScoped<InvocationCounter>();               

        #endregion
    }
}

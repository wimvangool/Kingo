using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MicroProcessorPipelineFactoryTest
    {
        #region [====== MessageHandlers ======]

        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 10)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 11)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 12)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 13)]
        private sealed class StringHandlerWithClassAndMethodFilters : IMessageHandler<string>
        {
            private readonly int _id;

            public StringHandlerWithClassAndMethodFilters(int id)
            {
                _id = id;
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public Task HandleAsync(string message, MessageHandlerContext context)
            {
                context.EventBus.Publish(_id);
                return Task.CompletedTask;
            }
        }

        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 10)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 11)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 12)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 13, OperationTypes = MicroProcessorOperationTypes.Query)]
        private sealed class StringHandlerWithDisabledClassAndMethodFilters : IMessageHandler<string>
        {
            private readonly int _id;

            public StringHandlerWithDisabledClassAndMethodFilters(int id)
            {
                _id = id;
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21, OperationTypes = MicroProcessorOperationTypes.None)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public Task HandleAsync(string message, MessageHandlerContext context)
            {
                context.EventBus.Publish(_id);
                return Task.CompletedTask;
            }
        }

        [FilterOne(MicroProcessorFilterStage.ProcessingStage, 1)]
        [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 2)]
        private sealed class StringHandlerWithDifferentFilterTypes : IMessageHandler<string>
        {
            private readonly int _id;

            public StringHandlerWithDifferentFilterTypes(int id)
            {
                _id = id;
            }

            [FilterOne(MicroProcessorFilterStage.ProcessingStage, 3)]
            [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 4)]
            public Task HandleAsync(string message, MessageHandlerContext context)
            {
                context.EventBus.Publish(_id);
                return Task.CompletedTask;
            }
        }

        #endregion

        #region [====== Queries ======]

        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 10)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 11)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 12)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 13)]
        private sealed class QueryWithClassAndMethodFilters : IQuery<object>, IQuery<object, object>
        {
            private readonly int _id;

            public QueryWithClassAndMethodFilters(int id)
            {
                _id = id;
            }                       

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public async Task<object> ExecuteAsync(QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public async Task<object> ExecuteAsync(object message, QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }
        }

        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 10, OperationTypes = MicroProcessorOperationTypes.None)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 11)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 12)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 13, OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler)]
        private sealed class QueryWithDisabledClassAndMethodFilters : IQuery<object>, IQuery<object, object>
        {
            private readonly int _id;

            public QueryWithDisabledClassAndMethodFilters(int id)
            {
                _id = id;
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21, OperationTypes = MicroProcessorOperationTypes.InputMessageHandler)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22, OperationTypes = MicroProcessorOperationTypes.MessageHandler)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public async Task<object> ExecuteAsync(QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21, OperationTypes = MicroProcessorOperationTypes.InputMessageHandler)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22, OperationTypes = MicroProcessorOperationTypes.MessageHandler)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public async Task<object> ExecuteAsync(object message, QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }
        }

        [FilterOne(MicroProcessorFilterStage.ProcessingStage, 1)]
        [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 2)]
        private sealed class QueryWithDifferentFilterTypes : IQuery<object>, IQuery<object, object>
        {
            private readonly int _id;

            public QueryWithDifferentFilterTypes(int id)
            {
                _id = id;
            }

            [FilterOne(MicroProcessorFilterStage.ProcessingStage, 3)]
            [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 4)]
            public async Task<object> ExecuteAsync(QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }

            [FilterOne(MicroProcessorFilterStage.ProcessingStage, 3)]
            [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 4)]
            public async Task<object> ExecuteAsync(object message, QueryContext context)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return new object();
            }
        }

        #endregion

        #region [====== FilterAttributes ======]

        private abstract class FilterAttribute : MicroProcessorFilterAttribute
        {
            private readonly int _id;

            protected FilterAttribute(MicroProcessorFilterStage stage, int id)
                : base(stage)
            {
                _id = id;
            }

            public override async Task<InvokeAsyncResult<MessageStream>> InvokeMessageHandlerAsync(MessageHandler handler)
            {
                handler.Context.EventBus.Publish(_id);
                return await handler.Method.InvokeAsync();
            }

            public override async Task<InvokeAsyncResult<TMessageOut>> InvokeQueryAsync<TMessageOut>(Query<TMessageOut> query)
            {
                await MicroServiceBusStub.Current.PublishAsync(_id);
                return await query.Method.InvokeAsync();
            }

        }

        private sealed class FilterOneAttribute : FilterAttribute
        {
            public FilterOneAttribute(MicroProcessorFilterStage stage, int id)
                : base(stage, id) { }
        }

        private sealed class FilterTwoAttribute : FilterAttribute
        {
            public FilterTwoAttribute(MicroProcessorFilterStage stage, int id)
                : base(stage, id) { }
        }

        #endregion        

        private readonly MessageHandlerFactoryBuilder _messageHandlers;
        private readonly MicroProcessorPipelineFactoryBuilder _pipeline;
        private readonly MicroServiceBusStub _serviceBus;

        public MicroProcessorPipelineFactoryTest()
        {
            _messageHandlers = new SimpleMessageHandlerFactoryBuilder();
            _pipeline = new MicroProcessorPipelineFactoryBuilder();
            _serviceBus = new MicroServiceBusStub();
        }

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfFilterIsNull()
        {
            _pipeline.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfFilterSpecifiedInvalidStage()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute((MicroProcessorFilterStage) (-1)));
        }

        #endregion

        #region [====== HandleAsync ======]

        [TestMethod]
        public async Task HandleAsync_ExecutesNoFilter_IfNoMessageHandlerIsInvoked()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage));
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage));

            await CreateProcessor().HandleAsync(new object());

            Assert.AreEqual(0, _serviceBus.Count);            
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(4);
            });

            Assert.AreEqual(5, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);
            Assert.AreEqual(1, _serviceBus[1]);
            Assert.AreEqual(2, _serviceBus[2]);
            Assert.AreEqual(3, _serviceBus[3]);
            Assert.AreEqual(4, _serviceBus[4]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2                
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                context.EventBus.Publish(4);
            });

            Assert.AreEqual(3, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);            
            Assert.AreEqual(2, _serviceBus[1]);            
            Assert.AreEqual(4, _serviceBus[2]);
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            _messageHandlers.Register(new StringHandlerWithClassAndMethodFilters(4));

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(13, _serviceBus.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _serviceBus[0]);
            Assert.AreEqual(10, _serviceBus[1]);
            Assert.AreEqual(20, _serviceBus[2]);

            // AuthorizationHandlingStage.
            Assert.AreEqual(1, _serviceBus[3]);
            Assert.AreEqual(11, _serviceBus[4]);
            Assert.AreEqual(21, _serviceBus[5]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _serviceBus[6]);
            Assert.AreEqual(12, _serviceBus[7]);
            Assert.AreEqual(22, _serviceBus[8]);

            // ProcessingStage.
            Assert.AreEqual(3, _serviceBus[9]);
            Assert.AreEqual(13, _serviceBus[10]);
            Assert.AreEqual(23, _serviceBus[11]);
            
            // Handler.
            Assert.AreEqual(4, _serviceBus[12]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.Input
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            _messageHandlers.Register(new StringHandlerWithDisabledClassAndMethodFilters(4));

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(9, _serviceBus.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _serviceBus[0]);
            Assert.AreEqual(10, _serviceBus[1]);
            Assert.AreEqual(20, _serviceBus[2]);

            // AuthorizationHandlingStage.            
            Assert.AreEqual(11, _serviceBus[3]);            

            // ValidationHandlingStage.            
            Assert.AreEqual(12, _serviceBus[4]);
            Assert.AreEqual(22, _serviceBus[5]);

            // ProcessingStage.
            Assert.AreEqual(3, _serviceBus[6]);            
            Assert.AreEqual(23, _serviceBus[7]);

            // Handler.
            Assert.AreEqual(4, _serviceBus[8]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {
            _messageHandlers.Register(new StringHandlerWithDifferentFilterTypes(5));

            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.Remove<FilterTwoAttribute>();

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(4, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);
            Assert.AreEqual(1, _serviceBus[1]);
            Assert.AreEqual(3, _serviceBus[2]);
            Assert.AreEqual(5, _serviceBus[3]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {
            _messageHandlers.Register(new StringHandlerWithDifferentFilterTypes(5));

            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.RemoveAllAttributes();

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(2, _serviceBus.Count);
            Assert.AreEqual(0, _serviceBus[0]);            
            Assert.AreEqual(5, _serviceBus[1]);
        }

        #endregion

        #region [====== ExecuteAsync1 ======]

        [TestMethod]
        public async Task ExecuteAsync1_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(async context =>
                {
                    await MicroServiceBusStub.Current.PublishAsync(4);
                });

                Assert.AreEqual(5, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(1, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(2, MicroServiceBusStub.Current[2]);
                Assert.AreEqual(3, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(4, MicroServiceBusStub.Current[4]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.InputMessageHandler
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Input
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(async context =>
                {
                    await MicroServiceBusStub.Current.PublishAsync(4);
                });

                Assert.AreEqual(3, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(1, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(2, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(4, MicroServiceBusStub.Current[2]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync1_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new QueryWithClassAndMethodFilters(4));

                Assert.AreEqual(13, MicroServiceBusStub.Current.Count);

                // ExceptionHandlingStage.
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(10, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(20, MicroServiceBusStub.Current[2]);

                // AuthorizationHandlingStage.
                Assert.AreEqual(1, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(11, MicroServiceBusStub.Current[4]);
                Assert.AreEqual(21, MicroServiceBusStub.Current[5]);

                // ValidationHandlingStage.
                Assert.AreEqual(2, MicroServiceBusStub.Current[6]);
                Assert.AreEqual(12, MicroServiceBusStub.Current[7]);
                Assert.AreEqual(22, MicroServiceBusStub.Current[8]);

                // ProcessingStage.
                Assert.AreEqual(3, MicroServiceBusStub.Current[9]);
                Assert.AreEqual(13, MicroServiceBusStub.Current[10]);
                Assert.AreEqual(23, MicroServiceBusStub.Current[11]);

                // Handler.
                Assert.AreEqual(4, MicroServiceBusStub.Current[12]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.Input
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });            

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new QueryWithDisabledClassAndMethodFilters(4));

                Assert.AreEqual(8, MicroServiceBusStub.Current.Count);

                // ExceptionHandlingStage.
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);                
                Assert.AreEqual(20, MicroServiceBusStub.Current[1]);

                // AuthorizationHandlingStage.                
                Assert.AreEqual(11, MicroServiceBusStub.Current[2]);                

                // ValidationHandlingStage.
                Assert.AreEqual(2, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(12, MicroServiceBusStub.Current[4]);                

                // ProcessingStage.
                Assert.AreEqual(3, MicroServiceBusStub.Current[5]);                
                Assert.AreEqual(23, MicroServiceBusStub.Current[6]);

                // Handler.
                Assert.AreEqual(4, MicroServiceBusStub.Current[7]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.Remove<FilterTwoAttribute>();

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new QueryWithDifferentFilterTypes(5));

                Assert.AreEqual(4, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(1, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(3, MicroServiceBusStub.Current[2]);
                Assert.AreEqual(5, MicroServiceBusStub.Current[3]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {            
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.RemoveAllAttributes();

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new QueryWithDifferentFilterTypes(5));

                Assert.AreEqual(2, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(5, MicroServiceBusStub.Current[1]);
            }
        }

        #endregion

        #region [====== ExecuteAsync2 ======]

        [TestMethod]
        public async Task ExecuteAsync2_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), async (message, context) =>
                {
                    await MicroServiceBusStub.Current.PublishAsync(4);
                });

                Assert.AreEqual(5, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(1, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(2, MicroServiceBusStub.Current[2]);
                Assert.AreEqual(3, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(4, MicroServiceBusStub.Current[4]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.InputMessageHandler
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Input
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), async (message, context) =>
                {
                    await MicroServiceBusStub.Current.PublishAsync(4);
                });

                Assert.AreEqual(3, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(1, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(2, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(4, MicroServiceBusStub.Current[2]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync2_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), new QueryWithClassAndMethodFilters(4));

                Assert.AreEqual(13, MicroServiceBusStub.Current.Count);

                // ExceptionHandlingStage.
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(10, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(20, MicroServiceBusStub.Current[2]);

                // AuthorizationHandlingStage.
                Assert.AreEqual(1, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(11, MicroServiceBusStub.Current[4]);
                Assert.AreEqual(21, MicroServiceBusStub.Current[5]);

                // ValidationHandlingStage.
                Assert.AreEqual(2, MicroServiceBusStub.Current[6]);
                Assert.AreEqual(12, MicroServiceBusStub.Current[7]);
                Assert.AreEqual(22, MicroServiceBusStub.Current[8]);

                // ProcessingStage.
                Assert.AreEqual(3, MicroServiceBusStub.Current[9]);
                Assert.AreEqual(13, MicroServiceBusStub.Current[10]);
                Assert.AreEqual(23, MicroServiceBusStub.Current[11]);

                // Handler.
                Assert.AreEqual(4, MicroServiceBusStub.Current[12]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.Input
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputMessageHandler
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), new QueryWithDisabledClassAndMethodFilters(4));

                Assert.AreEqual(8, MicroServiceBusStub.Current.Count);

                // ExceptionHandlingStage.
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(20, MicroServiceBusStub.Current[1]);

                // AuthorizationHandlingStage.                
                Assert.AreEqual(11, MicroServiceBusStub.Current[2]);

                // ValidationHandlingStage.
                Assert.AreEqual(2, MicroServiceBusStub.Current[3]);
                Assert.AreEqual(12, MicroServiceBusStub.Current[4]);

                // ProcessingStage.
                Assert.AreEqual(3, MicroServiceBusStub.Current[5]);
                Assert.AreEqual(23, MicroServiceBusStub.Current[6]);

                // Handler.
                Assert.AreEqual(4, MicroServiceBusStub.Current[7]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.Remove<FilterTwoAttribute>();

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), new QueryWithDifferentFilterTypes(5));

                Assert.AreEqual(4, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(1, MicroServiceBusStub.Current[1]);
                Assert.AreEqual(3, MicroServiceBusStub.Current[2]);
                Assert.AreEqual(5, MicroServiceBusStub.Current[3]);
            }
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {
            _pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _pipeline.RemoveAllAttributes();

            using (MicroServiceBusStub.CreateContext())
            {
                await CreateProcessor().ExecuteAsync(new object(), new QueryWithDifferentFilterTypes(5));

                Assert.AreEqual(2, MicroServiceBusStub.Current.Count);
                Assert.AreEqual(0, MicroServiceBusStub.Current[0]);
                Assert.AreEqual(5, MicroServiceBusStub.Current[1]);
            }
        }

        #endregion

        private MicroProcessor CreateProcessor() =>
            new MicroProcessor(_messageHandlers.Build(), _pipeline.Build(), _serviceBus);
    }
}

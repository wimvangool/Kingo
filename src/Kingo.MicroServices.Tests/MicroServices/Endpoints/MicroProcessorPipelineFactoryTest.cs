using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
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
                AddIdentifier(context);
                return Task.CompletedTask;
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
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
                AddIdentifier(context);
                return Task.CompletedTask;
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
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
                AddIdentifier(context);
                return Task.CompletedTask;
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
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
            public Task<object> ExecuteAsync(QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public Task<object> ExecuteAsync(object message, QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
        }

        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 10, OperationTypes = MicroProcessorOperationTypes.None)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 11)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 12)]
        [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 13, OperationTypes = MicroProcessorOperationTypes.OutputStream)]
        private sealed class QueryWithDisabledClassAndMethodFilters : IQuery<object>, IQuery<object, object>
        {
            private readonly int _id;

            public QueryWithDisabledClassAndMethodFilters(int id)
            {
                _id = id;
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21, OperationTypes = MicroProcessorOperationTypes.InputStream)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22, OperationTypes = MicroProcessorOperationTypes.AnyStream)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public Task<object> ExecuteAsync(QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ExceptionHandlingStage, Id = 20)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.AuthorizationStage, Id = 21, OperationTypes = MicroProcessorOperationTypes.InputStream)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ValidationStage, Id = 22, OperationTypes = MicroProcessorOperationTypes.AnyStream)]
            [MicroProcessorFilterSpy(MicroProcessorFilterStage.ProcessingStage, Id = 23)]
            public Task<object> ExecuteAsync(object message, QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
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
            public Task<object> ExecuteAsync(QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            [FilterOne(MicroProcessorFilterStage.ProcessingStage, 3)]
            [FilterTwo(MicroProcessorFilterStage.ProcessingStage, 4)]
            public Task<object> ExecuteAsync(object message, QueryContext context)
            {
                AddIdentifier(context);
                return Task.FromResult(new object());
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
        }

        #endregion

        #region [====== FilterAttributes ======]

        private abstract class FilterAttribute : MicroProcessorFilterAttribute
        {
            private readonly int _id;

            protected FilterAttribute(MicroProcessorFilterStage stage, int id) :
                base(stage)
            {
                _id = id;
            }

            public override async Task<HandleAsyncResult> InvokeMessageHandlerAsync(MessageHandler handler)
            {
                AddIdentifier(handler.Context);

                return await handler.Method.InvokeAsync();
            }

            public override async Task<ExecuteAsyncResult<TResponse>> InvokeQueryAsync<TResponse>(Query<TResponse> query)
            {
                AddIdentifier(query.Context);

                return await query.Method.InvokeAsync();
            }

            private void AddIdentifier(MicroProcessorContext context) =>
                context.ServiceProvider.GetRequiredService<ICollection<int>>().Add(_id);
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

        private readonly MicroProcessorBuilder<MicroProcessor> _processor;                
        private readonly List<int> _identifiers;

        public MicroProcessorPipelineFactoryTest()
        {
            _processor = new MicroProcessorBuilder<MicroProcessor>();                        
            _identifiers = new List<int>();
        }

        private IMicroProcessor CreateProcessor() =>
            _processor
                .BuildServiceCollection(new ServiceCollection().AddSingleton<ICollection<int>>(_identifiers))
                .BuildServiceProvider(true)
                .GetRequiredService<IMicroProcessor>();

        #region [====== Add ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Throws_IfFilterIsNull()
        {
            _processor.Pipeline.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfFilterSpecifiedInvalidStage()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute((MicroProcessorFilterStage) (-1)));
        }

        #endregion

        #region [====== HandleAsync ======]

        [TestMethod]
        public async Task HandleAsync_ExecutesNoFilter_IfNoMessageHandlerIsInvoked()
        {            
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage));
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage));
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage));
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage));

            await CreateProcessor().HandleAsync(new object());

            Assert.AreEqual(0, _identifiers.Count);            
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {            
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                _identifiers.Add(4);
                return Task.CompletedTask;
            });

            Assert.AreEqual(5, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(2, _identifiers[2]);
            Assert.AreEqual(3, _identifiers[3]);
            Assert.AreEqual(4, _identifiers[4]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {            
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2                
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });

            await CreateProcessor().HandleAsync(new object(), (message, context) =>
            {
                _identifiers.Add(4);
            });

            Assert.AreEqual(3, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);            
            Assert.AreEqual(2, _identifiers[1]);            
            Assert.AreEqual(4, _identifiers[2]);
        }

        [TestMethod]
        public async Task HandleAsync_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            _processor.Components.AddMessageHandler(new StringHandlerWithClassAndMethodFilters(4));

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(13, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(10, _identifiers[1]);
            Assert.AreEqual(20, _identifiers[2]);

            // AuthorizationHandlingStage.
            Assert.AreEqual(1, _identifiers[3]);
            Assert.AreEqual(11, _identifiers[4]);
            Assert.AreEqual(21, _identifiers[5]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _identifiers[6]);
            Assert.AreEqual(12, _identifiers[7]);
            Assert.AreEqual(22, _identifiers[8]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[9]);
            Assert.AreEqual(13, _identifiers[10]);
            Assert.AreEqual(23, _identifiers[11]);
            
            // Handler.
            Assert.AreEqual(4, _identifiers[12]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.AnyInput
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            _processor.Components.AddMessageHandler(new StringHandlerWithDisabledClassAndMethodFilters(4));

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(9, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(10, _identifiers[1]);
            Assert.AreEqual(20, _identifiers[2]);

            // AuthorizationHandlingStage.            
            Assert.AreEqual(11, _identifiers[3]);            

            // ValidationHandlingStage.            
            Assert.AreEqual(12, _identifiers[4]);
            Assert.AreEqual(22, _identifiers[5]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[6]);            
            Assert.AreEqual(23, _identifiers[7]);

            // Handler.
            Assert.AreEqual(4, _identifiers[8]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {
            _processor.Components.AddMessageHandler(new StringHandlerWithDifferentFilterTypes(5));

            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Remove<FilterTwoAttribute>();

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(4, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(3, _identifiers[2]);
            Assert.AreEqual(5, _identifiers[3]);
        }

        [TestMethod]
        public async Task HandleAsync_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {
            _processor.Components.AddMessageHandler(new StringHandlerWithDifferentFilterTypes(5));

            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.RemoveAllAttributes();

            await CreateProcessor().HandleAsync(string.Empty);

            Assert.AreEqual(2, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);            
            Assert.AreEqual(5, _identifiers[1]);
        }

        #endregion

        #region [====== ExecuteAsync1 ======]

        [TestMethod]
        public async Task ExecuteAsync1_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(context =>
            {
                _identifiers.Add(4);
                return Task.FromResult(new object());
            });

            Assert.AreEqual(5, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(2, _identifiers[2]);
            Assert.AreEqual(3, _identifiers[3]);
            Assert.AreEqual(4, _identifiers[4]);
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.InputStream
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.AnyInput
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });

            await CreateProcessor().ExecuteAsync( context =>
            {
                _identifiers.Add(4);                
                return Task.FromResult(new object());
            });

            Assert.AreEqual(3, _identifiers.Count);
            Assert.AreEqual(1, _identifiers[0]);
            Assert.AreEqual(2, _identifiers[1]);
            Assert.AreEqual(4, _identifiers[2]);
        }

        [TestMethod]
        public async Task ExecuteAsync1_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(new QueryWithClassAndMethodFilters(4));

            Assert.AreEqual(13, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(10, _identifiers[1]);
            Assert.AreEqual(20, _identifiers[2]);

            // AuthorizationHandlingStage.
            Assert.AreEqual(1, _identifiers[3]);
            Assert.AreEqual(11, _identifiers[4]);
            Assert.AreEqual(21, _identifiers[5]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _identifiers[6]);
            Assert.AreEqual(12, _identifiers[7]);
            Assert.AreEqual(22, _identifiers[8]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[9]);
            Assert.AreEqual(13, _identifiers[10]);
            Assert.AreEqual(23, _identifiers[11]);

            // Handler.
            Assert.AreEqual(4, _identifiers[12]);
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.AnyInput
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(new QueryWithDisabledClassAndMethodFilters(4));

            Assert.AreEqual(8, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(20, _identifiers[1]);

            // AuthorizationHandlingStage.                
            Assert.AreEqual(11, _identifiers[2]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _identifiers[3]);
            Assert.AreEqual(12, _identifiers[4]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[5]);
            Assert.AreEqual(23, _identifiers[6]);

            // Handler.
            Assert.AreEqual(4, _identifiers[7]);
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {            
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Remove<FilterTwoAttribute>();

            await CreateProcessor().ExecuteAsync(new QueryWithDifferentFilterTypes(5));

            Assert.AreEqual(4, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(3, _identifiers[2]);
            Assert.AreEqual(5, _identifiers[3]);
        }

        [TestMethod]
        public async Task ExecuteAsync1_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {            
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.RemoveAllAttributes();

            await CreateProcessor().ExecuteAsync(new QueryWithDifferentFilterTypes(5));

            Assert.AreEqual(2, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(5, _identifiers[1]);
        }

        #endregion

        #region [====== ExecuteAsync2 ======]

        [TestMethod]
        public async Task ExecuteAsync2_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(new object(), (message, context) =>
            {
                _identifiers.Add(4);
                return Task.FromResult(new object());
            });

            Assert.AreEqual(5, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(2, _identifiers[2]);
            Assert.AreEqual(3, _identifiers[3]);
            Assert.AreEqual(4, _identifiers[4]);
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.InputStream
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.AnyInput
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });

            await CreateProcessor().ExecuteAsync(new object(), (message, context) =>
            {
                _identifiers.Add(4);
                return Task.FromResult(new object());
            });

            Assert.AreEqual(3, _identifiers.Count);
            Assert.AreEqual(1, _identifiers[0]);
            Assert.AreEqual(2, _identifiers[1]);
            Assert.AreEqual(4, _identifiers[2]);
        }

        [TestMethod]
        public async Task ExecuteAsync2_ExecutesEveryFilterInExpectedOrder_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(new object(), new QueryWithClassAndMethodFilters(4));

            Assert.AreEqual(13, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(10, _identifiers[1]);
            Assert.AreEqual(20, _identifiers[2]);

            // AuthorizationHandlingStage.
            Assert.AreEqual(1, _identifiers[3]);
            Assert.AreEqual(11, _identifiers[4]);
            Assert.AreEqual(21, _identifiers[5]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _identifiers[6]);
            Assert.AreEqual(12, _identifiers[7]);
            Assert.AreEqual(22, _identifiers[8]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[9]);
            Assert.AreEqual(13, _identifiers[10]);
            Assert.AreEqual(23, _identifiers[11]);

            // Handler.
            Assert.AreEqual(4, _identifiers[12]);
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyExecutesEnabledFilters_IfFiltersAreAddedExplicitly_And_Implicitly()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ExceptionHandlingStage)
            {
                Id = 0,
                OperationTypes = MicroProcessorOperationTypes.AnyInput
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.AuthorizationStage)
            {
                Id = 1,
                OperationTypes = MicroProcessorOperationTypes.OutputStream
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ValidationStage)
            {
                Id = 2,
                OperationTypes = MicroProcessorOperationTypes.Query
            });
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 3
            });

            await CreateProcessor().ExecuteAsync(new object(), new QueryWithDisabledClassAndMethodFilters(4));

            Assert.AreEqual(8, _identifiers.Count);

            // ExceptionHandlingStage.
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(20, _identifiers[1]);

            // AuthorizationHandlingStage.                
            Assert.AreEqual(11, _identifiers[2]);

            // ValidationHandlingStage.
            Assert.AreEqual(2, _identifiers[3]);
            Assert.AreEqual(12, _identifiers[4]);

            // ProcessingStage.
            Assert.AreEqual(3, _identifiers[5]);
            Assert.AreEqual(23, _identifiers[6]);

            // Handler.
            Assert.AreEqual(4, _identifiers[7]);
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyInvokesSelectedFilters_IfSomeFiltersAreRemoved()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.Remove<FilterTwoAttribute>();

            await CreateProcessor().ExecuteAsync(new object(), new QueryWithDifferentFilterTypes(5));

            Assert.AreEqual(4, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(1, _identifiers[1]);
            Assert.AreEqual(3, _identifiers[2]);
            Assert.AreEqual(5, _identifiers[3]);
        }

        [TestMethod]
        public async Task ExecuteAsync2_OnlyInvokesExplicitFilters_IfAllImplicitFiltersAreRemoved()
        {
            _processor.Pipeline.Add(new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage)
            {
                Id = 0
            });
            _processor.Pipeline.RemoveAllAttributes();

            await CreateProcessor().ExecuteAsync(new object(), new QueryWithDifferentFilterTypes(5));

            Assert.AreEqual(2, _identifiers.Count);
            Assert.AreEqual(0, _identifiers[0]);
            Assert.AreEqual(5, _identifiers[1]);
        }

        #endregion        
    }
}

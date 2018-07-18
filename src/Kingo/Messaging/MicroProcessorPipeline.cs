using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a message pipeline that consists of all filters that were added
    /// globally, declared on the class-level or declared on the method-level of the message handler or query that
    /// is being invoked.
    /// </summary>
    public sealed class MicroProcessorPipeline : IEnumerable<MicroProcessorFilterAttribute>
    {
        #region [====== AddFilterFunction ======]

        private sealed class AddFilterFunction : IMicroProcessorFilterAttributeVisitor
        {
            private readonly MicroProcessorPipeline _pipeline;

            public AddFilterFunction(MicroProcessorPipeline pipeline)
            {
                _pipeline = pipeline;
            }

            public MicroProcessorPipeline Invoke(MicroProcessorFilterAttribute filter)
            {
                if (filter == null)
                {
                    throw new ArgumentNullException(nameof(filter));
                }
                filter.Accept(this);
                return _pipeline;
            }

            void IMicroProcessorFilterAttributeVisitor.Visit(ExceptionHandlingFilterAttribute filter) =>
                _pipeline._exceptionHandlingStage.Add(filter);

            void IMicroProcessorFilterAttributeVisitor.Visit(AuthorizationFilterAttribute filter) =>
                _pipeline._authorizationStage.Add(filter);

            void IMicroProcessorFilterAttributeVisitor.Visit(ValidationFilterAttribute filter) =>
                _pipeline._validationStage.Add(filter);

            void IMicroProcessorFilterAttributeVisitor.Visit(ProcessingFilterAttribute filter) =>
                _pipeline._processingStage.Add(filter);
        }

        #endregion

        #region [====== Stage ======]

        private sealed class Stage : IEnumerable<MicroProcessorFilterAttribute>
        {
            private readonly Stage _previousLevel;
            private readonly List<MicroProcessorFilterAttribute> _filters;

            public Stage() :
                this(null) { }
            
            private Stage(Stage previousLevel)
            {
                _previousLevel = previousLevel;
                _filters = new List<MicroProcessorFilterAttribute>();
            }

            public Stage NextLevel() =>
                new Stage(this);

            public void Add(MicroProcessorFilterAttribute filter) =>
                _filters.Add(filter);

            public IEnumerator<MicroProcessorFilterAttribute> GetEnumerator() =>
                PreviousLevelFilters().Concat(_filters).GetEnumerator();

            private IEnumerable<MicroProcessorFilterAttribute> PreviousLevelFilters() =>
                _previousLevel ?? Enumerable.Empty<MicroProcessorFilterAttribute>();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        #endregion

        private readonly AddFilterFunction _addFilterFunction;
        private readonly Stage _exceptionHandlingStage;
        private readonly Stage _authorizationStage;
        private readonly Stage _validationStage;
        private readonly Stage _processingStage;
        private readonly bool _disableClassAndMethodAttributes;

        internal MicroProcessorPipeline() :
            this(new Stage(), new Stage(), new Stage(), new Stage(), false) { }

        private MicroProcessorPipeline(Stage exceptionHandlingStage, Stage authorizationStage, Stage validationStage, Stage processingStage, bool disableClassAndMethodAttributes)
        {
            _addFilterFunction = new AddFilterFunction(this);
            _exceptionHandlingStage = exceptionHandlingStage;
            _authorizationStage = authorizationStage;
            _validationStage = validationStage;
            _processingStage = processingStage;
            _disableClassAndMethodAttributes = disableClassAndMethodAttributes;
        }

        #region [====== ToString ======]

        private const string _FilterSeparator = " | ";

        /// <inheritdoc />
        public override string ToString() =>
            string.Join(_FilterSeparator, this.Select(filter => ToString(filter)));

        internal static string ToString(object pipelineComponent) =>
            pipelineComponent.GetType().FriendlyName();

        internal static string ToString(object leftComponent, object rightComponent) =>
            leftComponent + _FilterSeparator + rightComponent;

        #endregion

        /// <inheritdoc />
        public IEnumerator<MicroProcessorFilterAttribute> GetEnumerator() =>
            _exceptionHandlingStage.Concat(_authorizationStage).Concat(_validationStage).Concat(_processingStage).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #region [====== MessageHandler ======]

        internal MessageHandler Build(MessageHandler handler) =>
            Build(handler, AddNextLevelFilters(handler));

        private static MessageHandler Build(MessageHandler handler, IEnumerable<MicroProcessorFilterAttribute> filters)
        {
            var pipeline = handler;

            foreach (var filter in filters.Reverse())
            {
                pipeline = new MessageHandlerConnector(pipeline, filter);
            }
            return pipeline;
        }

        #endregion

        #region [====== Query<TMessageOut> ======]

        internal Query<TMessageOut> Build<TMessageOut>(Query<TMessageOut> query) =>
            Build(query, AddNextLevelFilters(query));

        private static Query<TMessageOut> Build<TMessageOut>(Query<TMessageOut> query, IEnumerable<MicroProcessorFilterAttribute> filters)
        {
            Query<TMessageOut> pipeline = query;

            foreach (var filter in filters.Reverse())
            {
                pipeline = new QueryConnector<TMessageOut>(pipeline, filter);
            }
            return pipeline;
        }

        #endregion       

        private MicroProcessorPipeline AddNextLevelFilters<TMessageHandlerOrQuery>(TMessageHandlerOrQuery messageHandlerOrQuery) where TMessageHandlerOrQuery : ITypeAttributeProvider, IMethodAttributeProvider =>
            AddClassLevelFilters(messageHandlerOrQuery).AddMethodLevelFilters(messageHandlerOrQuery);

        private MicroProcessorPipeline AddClassLevelFilters(ITypeAttributeProvider classAttributeProvider) =>
            NextLevel().Add(_disableClassAndMethodAttributes ? Enumerable.Empty<MicroProcessorFilterAttribute>() : classAttributeProvider.GetTypeAttributesOfType<MicroProcessorFilterAttribute>());

        private MicroProcessorPipeline AddMethodLevelFilters(IMethodAttributeProvider methodAttributeProvider) =>
            NextLevel().Add(_disableClassAndMethodAttributes ? Enumerable.Empty<MicroProcessorFilterAttribute>() : methodAttributeProvider.GetMethodAttributesOfType<MicroProcessorFilterAttribute>());

        private MicroProcessorPipeline NextLevel() =>
            new MicroProcessorPipeline(_exceptionHandlingStage.NextLevel(), _authorizationStage.NextLevel(), _validationStage.NextLevel(), _processingStage.NextLevel(), _disableClassAndMethodAttributes);

        /// <summary>
        /// Disables all <see cref="MicroProcessorFilterAttribute">filters</see> that were declared as <see cref="Attribute" /> on message handlers and queries.
        /// This can be useful to prevent any code related to security, logging and/or caching to run while running tests that are focussed on the
        /// functional aspects of your code.
        /// </summary>
        /// <returns></returns>
        public MicroProcessorPipeline DisableClassAndMethodAttributes() =>
            new MicroProcessorPipeline(_exceptionHandlingStage, _authorizationStage, _validationStage, _processingStage, true);

        /// <summary>
        /// Adds a collection of filters to this pipeline.
        /// </summary>
        /// <param name="filters">A collection of filters.</param>
        /// <returns>A pipeline where all specified filters are part of.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filters"/> is <c>null</c>.
        /// </exception>
        public MicroProcessorPipeline Add(IEnumerable<MicroProcessorFilterAttribute> filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }
            var pipeline = this;

            foreach (var filter in filters.WhereNotNull())
            {
                pipeline = pipeline.Add(filter);
            }
            return pipeline;
        }

        /// <summary>
        /// Adds the specified <paramref name="filter"/> to the pipeline.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>This pipeline, with the filter added to its configuration.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="filter"/> specifies an invalid stage.
        /// </exception>
        public MicroProcessorPipeline Add(MicroProcessorFilterAttribute filter) =>
            _addFilterFunction.Invoke(filter);        
    }
}

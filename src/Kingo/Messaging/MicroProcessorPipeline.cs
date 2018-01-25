using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a message pipeline that consists of all filters that were added
    /// globally, declared on the class-level or declared on the method-level of the message handler or query that
    /// is being invoked.
    /// </summary>
    public sealed class MicroProcessorPipeline : IEnumerable<IMicroProcessorFilter>
    {
        #region [====== Stage ======]

        private sealed class Stage : IEnumerable<IMicroProcessorFilter>
        {
            private readonly Stage _previousLevel;
            private readonly LinkedList<IMicroProcessorFilter> _filters;

            public Stage() :
                this(null) { }
            
            private Stage(Stage previousLevel)
            {
                _previousLevel = previousLevel;
                _filters = new LinkedList<IMicroProcessorFilter>();
            }

            public Stage NextLevel() =>
                new Stage(this);

            public void Add(IMicroProcessorFilter filter)
            {
                if (MustBeAddedBeforeOtherFilter(filter.StagePosition, out LinkedListNode<IMicroProcessorFilter> otherFilter))
                {
                    _filters.AddBefore(otherFilter, filter);
                }
                else
                {
                    _filters.AddLast(filter);
                }
            }

            private bool MustBeAddedBeforeOtherFilter(byte position, out LinkedListNode<IMicroProcessorFilter> otherFilter)
            {
                if (_filters.Count > 0)
                {
                    var filter = _filters.First;

                    do
                    {
                        if (position < filter.Value.StagePosition)
                        {
                            otherFilter = filter;
                            return true;
                        }
                    } while ((filter = filter.Next) != null);
                }
                otherFilter = null;
                return false;
            }

            public IEnumerator<IMicroProcessorFilter> GetEnumerator() =>
                PreviousLevelFilters().Concat(_filters).GetEnumerator();

            private IEnumerable<IMicroProcessorFilter> PreviousLevelFilters() =>
                _previousLevel ?? Enumerable.Empty<IMicroProcessorFilter>();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        #endregion

        private readonly Stage _exceptionHandlingStage;
        private readonly Stage _authorizationStage;
        private readonly Stage _validationStage;
        private readonly Stage _processingStage;

        internal MicroProcessorPipeline() :
            this(new Stage(), new Stage(), new Stage(), new Stage()) { }

        private MicroProcessorPipeline(Stage exceptionHandlingStage, Stage authorizationStage, Stage validationStage, Stage processingStage)
        {
            _exceptionHandlingStage = exceptionHandlingStage;
            _authorizationStage = authorizationStage;
            _validationStage = validationStage;
            _processingStage = processingStage;
        }

        /// <inheritdoc />
        public override string ToString() =>
            string.Join(" | ", this.Select(filter => filter.GetType().FriendlyName()));

        /// <inheritdoc />
        public IEnumerator<IMicroProcessorFilter> GetEnumerator() =>
            _exceptionHandlingStage.Concat(_authorizationStage).Concat(_validationStage).Concat(_processingStage).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #region [====== MessageHandler<TMessage> ======]

        internal MessageHandler<TMessage> Build<TMessage>(MessageHandler<TMessage> handler) =>
            Build(handler, AddNextLevelFilters(handler));

        private static MessageHandler<TMessage> Build<TMessage>(MessageHandler<TMessage> handler, IEnumerable<IMicroProcessorFilter> filters)
        {
            var pipeline = handler;

            foreach (var filter in filters.Reverse())
            {
                pipeline = new MessageHandlerConnector<TMessage>(pipeline, filter);
            }
            return pipeline;
        }

        #endregion

        #region [====== Query<TMessageOut> ======]

        internal Query<TMessageOut> Build<TMessageOut>(Query<TMessageOut> query) =>
            Build(query, AddNextLevelFilters(query));

        private static Query<TMessageOut> Build<TMessageOut>(Query<TMessageOut> query, IEnumerable<IMicroProcessorFilter> filters)
        {
            Query<TMessageOut> pipeline = query;

            foreach (var filter in filters.Reverse())
            {
                pipeline = new QueryConnector<TMessageOut>(pipeline, filter);
            }
            return pipeline;
        }

        #endregion

        #region [====== Query<TMessageIn, TMessageOut> ======]

        internal Query<TMessageIn, TMessageOut> Build<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query) =>
            Build(query, AddNextLevelFilters(query));

        private static Query<TMessageIn, TMessageOut> Build<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, IEnumerable<IMicroProcessorFilter> filters)
        {
            Query<TMessageIn, TMessageOut> pipeline = query;

            foreach (var filter in filters.Reverse())
            {
                pipeline = new QueryConnector<TMessageIn, TMessageOut>(pipeline, filter);
            }
            return pipeline;
        }

        #endregion

        private MicroProcessorPipeline AddNextLevelFilters<TMessageHandlerOrQuery>(TMessageHandlerOrQuery messageHandlerOrQuery) where TMessageHandlerOrQuery : ITypeAttributeProvider, IMethodAttributeProvider =>
            AddClassLevelFilters(messageHandlerOrQuery).AddMethodLevelFilters(messageHandlerOrQuery);

        private MicroProcessorPipeline AddClassLevelFilters(ITypeAttributeProvider classAttributeProvider) =>
            NextLevel().Add(classAttributeProvider.GetTypeAttributesOfType<IMicroProcessorFilter>());

        private MicroProcessorPipeline AddMethodLevelFilters(IMethodAttributeProvider methodAttributeProvider) =>
            NextLevel().Add(methodAttributeProvider.GetMethodAttributesOfType<IMicroProcessorFilter>());

        private MicroProcessorPipeline NextLevel() =>
            new MicroProcessorPipeline(_exceptionHandlingStage.NextLevel(), _authorizationStage.NextLevel(), _validationStage.NextLevel(), _processingStage.NextLevel());

        /// <summary>
        /// Adds a collection of filters to this pipeline.
        /// </summary>
        /// <param name="filters">A collection of filters.</param>
        /// <returns>A pipeline where all specified filters are part of.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filters"/> is <c>null</c>.
        /// </exception>
        public MicroProcessorPipeline Add(IEnumerable<IMicroProcessorFilter> filters)
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
        public MicroProcessorPipeline Add(IMicroProcessorFilter filter)
        {
            StageOf(filter).Add(filter);
            return this;
        } 
        
        private Stage StageOf(IMicroProcessorFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            switch (filter.Stage)
            {
                case MicroProcessorPipelineStage.ExceptionHandlingStage:
                    return _exceptionHandlingStage;
                case MicroProcessorPipelineStage.AuthorizationStage:
                    return _authorizationStage;
                case MicroProcessorPipelineStage.ValidationStage:
                    return _validationStage;
                case MicroProcessorPipelineStage.ProcessingStage:
                    return _processingStage;
                default:
                    throw NewInvalidStageException(filter);
            }
        }

        private static Exception NewInvalidStageException(IMicroProcessorFilter filter)
        {
            var messageFormat = ExceptionMessages.MicroProcessorPipeline_InvalidStage;
            var message = string.Format(messageFormat, filter.GetType().FriendlyName(), filter.Stage);
            return new ArgumentException(message, nameof(filter));
        }        
    }
}

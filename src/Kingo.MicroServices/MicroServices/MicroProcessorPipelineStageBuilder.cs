using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.MicroServices.Configuration;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorPipelineStageBuilder : IMicroProcessorPipelineFactoryBuilder
    {
        #region [====== MicroProcessorFilterCollection ======]

        private abstract class MicroProcessorFilterCollection : IMicroProcessorPipelineFactory
        {
            public MessageHandler CreatePipeline(MessageHandler handler)
            {
                foreach (var filter in GetEnabledFilters(handler))
                {
                    handler = new MessageHandlerFilterPipeline(handler, filter);
                }
                return handler;
            }

            public Query<TResponse> CreatePipeline<TResponse>(Query<TResponse> query)
            {
                foreach (var filter in GetEnabledFilters(query))
                {
                    query = new QueryFilterPipeline<TResponse>(query, filter);
                }
                return query;
            }

            private IEnumerable<IMicroProcessorFilter> GetEnabledFilters<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery) =>
                GetFilters(messageHandlerOrQuery).Where(filter => filter.IsEnabled(messageHandlerOrQuery.Context));

            protected abstract IEnumerable<IMicroProcessorFilter> GetFilters<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery);
        }

        #endregion

        #region [====== GlobalFilterCollection ======]

        private sealed class GlobalFilterCollection : MicroProcessorFilterCollection
        {
            private readonly IMicroProcessorFilter[] _filters;

            public GlobalFilterCollection(IEnumerable<IMicroProcessorFilter> filters)
            {
                _filters = filters.Reverse().ToArray();
            }

            protected override IEnumerable<IMicroProcessorFilter> GetFilters<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery) =>
                _filters;
        }

        #endregion

        #region [====== ClassOrMethodLevelFilterCollection ======]

        private abstract class ClassOrMethodLevelFilterCollection<TMember> : MicroProcessorFilterCollection
            where TMember : MemberInfo
        {
            private readonly ConcurrentDictionary<TMember, IMicroProcessorFilter[]> _filtersPerMember;            

            protected ClassOrMethodLevelFilterCollection()
            {
                _filtersPerMember = new ConcurrentDictionary<TMember, IMicroProcessorFilter[]>();
            }

            protected abstract MicroProcessorFilterStage Stage
            {
                get;
            }

            protected abstract IRemovedAttributeCollection RemovedAttributeCollection
            {
                get;
            }

            protected override IEnumerable<IMicroProcessorFilter> GetFilters<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery) =>
                GetOrAddFilters(SelectAttributeProvider(messageHandlerOrQuery));

            private IEnumerable<IMicroProcessorFilter> GetOrAddFilters(IAttributeProvider<TMember> attributeProvider) =>
                _filtersPerMember.GetOrAdd(attributeProvider.Target, member => GetFilters(attributeProvider).ToArray());

            private IEnumerable<IMicroProcessorFilter> GetFilters(IAttributeProvider<TMember> attributeProvider) =>
                from filter in attributeProvider.GetAttributesOfType<IMicroProcessorFilter>()
                where filter.Stage == Stage && !RemovedAttributeCollection.Contains(filter.GetType())
                select filter;

            protected abstract IAttributeProvider<TMember> SelectAttributeProvider<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery);
        }

        #endregion

        #region [====== ClassLevelFilterCollection ======]

        private sealed class ClassLevelFilterCollection : ClassOrMethodLevelFilterCollection<Type>
        {            
            public ClassLevelFilterCollection(MicroProcessorFilterStage stage, IRemovedAttributeCollection removedAttributeCollection)
            {
                Stage = stage;
                RemovedAttributeCollection = removedAttributeCollection;
            }            

            protected override MicroProcessorFilterStage Stage
            {
                get;
            }

            protected override IRemovedAttributeCollection RemovedAttributeCollection
            {
                get;
            }

            protected override IAttributeProvider<Type> SelectAttributeProvider<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery) =>
                messageHandlerOrQuery;
        }

        #endregion

        #region [====== MethodLevelFilterCollection ======]

        private sealed class MethodLevelFilterCollection : ClassOrMethodLevelFilterCollection<MethodInfo>
        {
            public MethodLevelFilterCollection(MicroProcessorFilterStage stage, IRemovedAttributeCollection removedAttributeCollection)
            {
                Stage = stage;
                RemovedAttributeCollection = removedAttributeCollection;
            }

            protected override MicroProcessorFilterStage Stage
            {
                get;
            }

            protected override IRemovedAttributeCollection RemovedAttributeCollection
            {
                get;
            }

            protected override IAttributeProvider<MethodInfo> SelectAttributeProvider<TResult>(IMessageHandlerOrQuery<TResult> messageHandlerOrQuery) =>
                messageHandlerOrQuery.Method;
        }

        #endregion

        private readonly MicroProcessorFilterStage _stage;
        private readonly List<IMicroProcessorFilter> _filters;
        private readonly RemovedAttributeCollection _attributes;

        public MicroProcessorPipelineStageBuilder(MicroProcessorFilterStage stage)
        {
            _stage = stage;
            _filters = new List<IMicroProcessorFilter>();
            _attributes = new RemovedAttributeCollection();
        }

        public void Add(IMicroProcessorFilter filter) =>
            _filters.Add(filter);

        public void Remove<TAttribute>() where TAttribute : Attribute, IMicroProcessorFilter =>
            _attributes.Remove<TAttribute>();

        public void RemoveAllAttributes() =>
            _attributes.RemoveAllAttributes();

        public IMicroProcessorPipelineFactory Build() =>
            new MicroProcessorPipelineFactory(FilterLevels());

        private IEnumerable<IMicroProcessorPipelineFactory> FilterLevels()
        {
            yield return CreateGlobalFilterCollection();
            yield return CreateClassLevelFilterCollection();
            yield return CreateMethodLevelFilterCollection();
        }

        private IMicroProcessorPipelineFactory CreateGlobalFilterCollection() =>
            new GlobalFilterCollection(_filters);

        private IMicroProcessorPipelineFactory CreateClassLevelFilterCollection() =>
            new ClassLevelFilterCollection(_stage, _attributes);

        private IMicroProcessorPipelineFactory CreateMethodLevelFilterCollection() =>
            new MethodLevelFilterCollection(_stage, _attributes);
    }
}

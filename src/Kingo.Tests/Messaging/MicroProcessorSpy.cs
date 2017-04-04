using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class MicroProcessorSpy : MicroProcessor
    {
        private readonly MessageHandlerImplementationSequence _implementationSequence;
        private readonly List<Type> _messageHandlers;
        private readonly List<IMicroProcessorPipeline> _pipeline;
        private readonly Dictionary<Type, object> _singletonDependencies;

        public MicroProcessorSpy()
        {
            _implementationSequence = new MessageHandlerImplementationSequence();
            _messageHandlers = new List<Type>();
            _pipeline = new List<IMicroProcessorPipeline>();
            _singletonDependencies = new Dictionary<Type, object>();
        }       

        public void Register<TMessageHandler>() =>
            _messageHandlers.Add(typeof(TMessageHandler));

        public MessageHandlerImplementation Implement<TMessageHander>()
        {
            var messageHandlerType = typeof(TMessageHander);

            _messageHandlers.Add(messageHandlerType);

            return _implementationSequence.Implement(messageHandlerType);
        }

        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = base.CreateMessageHandlerFactory();
                
            foreach (var dependency in _singletonDependencies)
            {
                factory = factory.RegisterSingleton(dependency.Key, dependency.Value);
            }
            return factory.RegisterSingleton<IMessageHandlerImplementation>(_implementationSequence);
        }

        protected override TypeSet CreateMessageHandlerTypeSet(TypeSet typeSet) =>
            typeSet.Add(_messageHandlers);

        public override Task<IMessageStream> HandleStreamAsync(IMessageStream inputStream, CancellationToken? token = null)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            return AsyncMethod.RunSynchronously(async () =>
            {
                try
                {
                    return await base.HandleStreamAsync(inputStream, token);
                }
                finally
                {
                    _implementationSequence.AssertNoMoreInvocationsExpected();
                }
            }, token);                       
        }

        public void Add(IMicroProcessorPipeline pipeline) =>
            _pipeline.Add(pipeline);

        protected internal override IEnumerable<IMicroProcessorPipeline> CreateProcessorPipeline() =>
            _pipeline;
    }
}

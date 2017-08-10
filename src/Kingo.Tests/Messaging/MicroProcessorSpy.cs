using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    public sealed class MicroProcessorSpy : MicroProcessor
    {
        private readonly MessageHandlerImplementationSequence _implementationSequence;
        private readonly List<Type> _messageHandlers;
        private readonly Dictionary<Type, Type> _dependencies;
        private readonly List<IMicroProcessorPipeline> _pipeline;        

        public MicroProcessorSpy()
        {
            _implementationSequence = new MessageHandlerImplementationSequence();
            _messageHandlers = new List<Type>();
            _dependencies = new Dictionary<Type, Type>();
            _pipeline = new List<IMicroProcessorPipeline>();            
        }       

        public void Register<TMessageHandler>() =>
            _messageHandlers.Add(typeof(TMessageHandler));

        public void RegisterDependency<TFrom, TTo>() where TTo : TFrom =>
            _dependencies.Add(typeof(TFrom), typeof(TTo));

        public MessageHandlerImplementation Implement<TMessageHander>(int count = 1)
        {
            var messageHandlerType = typeof(TMessageHander);

            _messageHandlers.Add(messageHandlerType);

            return _implementationSequence.Implement(messageHandlerType, count);
        }

        protected internal override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            MessageHandlerFactory factory = new UnityContainerFactory();

            foreach (var dependency in _dependencies)
            {
                factory = factory.Register(dependency.Key, dependency.Value);
            }
            return factory.RegisterInstance<IMessageHandlerImplementation>(_implementationSequence);
        }
            
        protected override TypeSet CreateMessageHandlerTypeSet() =>
            TypeSet.Empty.Add(_messageHandlers);

        public override Task<IMessageStream> HandleStreamAsync(IMessageStream inputStream, CancellationToken? token = null)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            return AsyncMethod.RunSynchronously(async () =>
            {
                var performFinalAssert = true;

                try
                {
                    return await base.HandleStreamAsync(inputStream, token);
                }        
                catch
                {
                    performFinalAssert = false;
                    throw;
                }        
                finally
                {
                    if (performFinalAssert)
                    {
                        _implementationSequence.AssertNoMoreInvocationsExpected();
                    }                    
                }
            }, token);                       
        }

        public void Add(IMicroProcessorPipeline pipeline) =>
            _pipeline.Add(pipeline);

        protected override IEnumerable<IMicroProcessorPipeline> CreateMessagePipeline() =>
            _pipeline;
    }
}

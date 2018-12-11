//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Kingo.MicroServices
//{
//    public sealed class MicroProcessorSpy : MicroProcessor
//    {
//        private readonly MessageHandlerImplementationSequence _implementationSequence;
//        private readonly List<Type> _messageHandlers;
//        private readonly Dictionary<Type, Type> _dependencies;
//        private readonly List<MicroProcessorFilterAttribute> _filters;        

//        public MicroProcessorSpy()
//        {
//            _implementationSequence = new MessageHandlerImplementationSequence();
//            _messageHandlers = new List<Type>();
//            _dependencies = new Dictionary<Type, Type>();
//            _filters = new List<MicroProcessorFilterAttribute>();            
//        }       

//        public void Register<TMessageHandler>() =>
//            _messageHandlers.Add(typeof(TMessageHandler));

//        public void RegisterDependency<TFrom, TTo>() where TTo : TFrom =>
//            _dependencies.Add(typeof(TFrom), typeof(TTo));

//        public MessageHandlerImplementation Implement<TMessageHander>(int count = 1)
//        {
//            var messageHandlerType = typeof(TMessageHander);

//            _messageHandlers.Add(messageHandlerType);

//            return _implementationSequence.Implement(messageHandlerType, count);
//        }

//        protected internal override MessageHandlerFactory BuildMessageHandlerFactory()
//        {
//            MessageHandlerFactory factory = null;//new UnityContainerFactory();

//            foreach (var dependency in _dependencies)
//            {
//                factory = factory.Register(dependency.Key, dependency.Value);
//            }
//            return factory
//                .RegisterInstance<IMessageHandlerImplementation>(_implementationSequence)
//                .Register(_messageHandlers);
//        }                    

//        protected override async Task<IMessageStream> HandleInputStreamAsync(HandleInputStreamMethod method)
//        {
//            var performFinalAssert = true;

//            try
//            {
//                return await base.HandleInputStreamAsync(method);
//            }
//            catch
//            {
//                performFinalAssert = false;
//                throw;
//            }
//            finally
//            {
//                if (performFinalAssert)
//                {
//                    _implementationSequence.AssertNoMoreInvocationsExpected();
//                }
//            }
//        }

//        public void Add(MicroProcessorFilterAttribute filter) =>
//            _filters.Add(filter);

//        protected override MicroProcessorPipeline BuildPipeline(MicroProcessorPipeline pipeline) =>
//            pipeline.Add(_filters);
//    }
//}

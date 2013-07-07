using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using YellowFlare.MessageProcessing.SampleApplication;
using YellowFlare.MessageProcessing.SampleApplication.Infrastructure;

namespace YellowFlare.MessageProcessing
{
    internal sealed class SampleApplicationProcessor : IMessageProcessor
    {
        private readonly MessageProcessor _core;

        private SampleApplicationProcessor(MessageHandlerFactory handlerFactory)
        {
            _core = new MessageProcessor(this, handlerFactory);
        }

        public IMessageProcessorBus Bus
        {
            get { return _core.Bus; }
        }

        public void Handle<TMessage>(TMessage message) where TMessage : class
        {
            _core.Handle(message);
        }

        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {
            _core.Handle(message, handler);
        }

        public void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            _core.Handle(message, action);
        }

        private static readonly Lazy<SampleApplicationProcessor> _Instance = new Lazy<SampleApplicationProcessor>(CreateProcessor, true);        

        public static SampleApplicationProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static SampleApplicationProcessor CreateProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity()
                .RegisterInstance(typeof(IUnitOfWorkController), UnitOfWorkContext.Controller)
                .RegisterType<IShoppingCartRepository, ShoppingCartRepository>()
                .RegisterType<ShoppingCartRepository>(new ContainerControlledLifetimeManager())
                .RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);

            return new SampleApplicationProcessor(messageHandlerFactory);
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace == "YellowFlare.MessageProcessing.SampleApplication.MessageHandlers";
        }
    }
}

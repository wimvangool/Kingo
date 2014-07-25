using System.ComponentModel.Messaging.Server.SampleApplication;
using System.ComponentModel.Messaging.Server.SampleApplication.Infrastructure;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace System.ComponentModel.Messaging.Server
{
    internal sealed class SampleApplicationProcessor : MessageProcessor
    {        
        private readonly MessageHandlerFactory _messageHandlerFactory;

        private SampleApplicationProcessor(MessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        private static readonly Lazy<SampleApplicationProcessor> _Instance = new Lazy<SampleApplicationProcessor>(CreateProcessor, true);        

        public static SampleApplicationProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static SampleApplicationProcessor CreateProcessor()
        {
            var factory = new MessageHandlerFactoryForUnity();

            factory.RegisterMessageHandlersFrom(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);
            factory.Container
                .RegisterType<IShoppingCartRepository, ShoppingCartRepository>()
                .RegisterType<ShoppingCartRepository>(new ContainerControlledLifetimeManager());                

            return new SampleApplicationProcessor(factory);
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace == "System.ComponentModel.Messaging.Server.SampleApplication.MessageHandlers";
        }
    }
}

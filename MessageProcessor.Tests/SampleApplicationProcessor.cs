using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using YellowFlare.MessageProcessing.SampleApplication;
using YellowFlare.MessageProcessing.SampleApplication.Infrastructure;

namespace YellowFlare.MessageProcessing
{
    internal sealed class SampleApplicationProcessor : MessageProcessor
    {        
        private SampleApplicationProcessor(MessageHandlerFactory handlerFactory) : base(handlerFactory) { }        

        private static readonly Lazy<SampleApplicationProcessor> _Instance = new Lazy<SampleApplicationProcessor>(CreateProcessor, true);        

        public static SampleApplicationProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static SampleApplicationProcessor CreateProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity()                
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

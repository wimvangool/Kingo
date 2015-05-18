using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Server
{
    internal sealed class SampleApplicationProcessor : MessageProcessor
    {        
        private readonly MessageHandlerFactory _messageHandlerFactory;

        private SampleApplicationProcessor(MessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected internal override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }

        private static readonly Lazy<SampleApplicationProcessor> _Instance = new Lazy<SampleApplicationProcessor>(CreateProcessor, true);        

        public static SampleApplicationProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static SampleApplicationProcessor CreateProcessor()
        {
            var factory = new UnityFactory();
            
            factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);
            factory.RegisterDependencies(Assembly.GetExecutingAssembly(), null, IsRepositoryInterface);
            
            return new SampleApplicationProcessor(factory);
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace == "System.ComponentModel.Server.SampleApplication.MessageHandlers";
        }

        private static bool IsRepositoryInterface(Type type)
        {
            return type.IsInterface && type.Name.EndsWith("Repository");
        }
    }
}

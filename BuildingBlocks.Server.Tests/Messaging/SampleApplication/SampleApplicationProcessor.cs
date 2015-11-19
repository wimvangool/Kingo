using System;
using System.Reflection;
using Kingo.BuildingBlocks.ComponentModel.Server;

namespace Kingo.BuildingBlocks.Messaging.SampleApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class SampleApplicationProcessor : MessageProcessor
    {
        private readonly UnityFactory _messageHandlerFactory;

        private SampleApplicationProcessor(UnityFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected internal override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        private static readonly Lazy<SampleApplicationProcessor> _Instance = new Lazy<SampleApplicationProcessor>(CreateProcessor, true);

        /// <summary>
        /// Returns the instance of <see cref="SampleApplicationProcessor" />.
        /// </summary>
        public static SampleApplicationProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static SampleApplicationProcessor CreateProcessor()
        {
            return new SampleApplicationProcessor(CreateMessageHandlerFactory());
        }
        
        private static UnityFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();

            factory.RegisterMessageHandlers(Assembly.GetExecutingAssembly(), IsHandlerForMessageProcessorTests);
            factory.RegisterDependencies(Assembly.GetExecutingAssembly(), null, IsRepositoryInterface);

            return factory;
        }

        private static bool IsHandlerForMessageProcessorTests(Type type)
        {
            return type.Namespace.EndsWith("SampleApplication.MessageHandlers");
        }

        private static bool IsRepositoryInterface(Type type)
        {
            return type.IsInterface && type.Name.EndsWith("Repository");
        }
    }
}
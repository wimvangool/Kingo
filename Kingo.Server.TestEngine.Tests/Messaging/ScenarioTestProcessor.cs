using System;
using System.Reflection;
using Kingo.ComponentModel.Server;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class ScenarioTestProcessor : MessageProcessor
    {
        private readonly UnityFactory _messageHandlerFactory;

        private ScenarioTestProcessor(UnityFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        private static readonly Lazy<ScenarioTestProcessor> _Instance = new Lazy<ScenarioTestProcessor>(CreateProcessor, true);

        /// <summary>
        /// Returns the instance of <see cref="ScenarioTestProcessor" />.
        /// </summary>
        public static ScenarioTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static ScenarioTestProcessor CreateProcessor()
        {
            return new ScenarioTestProcessor(CreateMessageHandlerFactory());
        }

        private static UnityFactory CreateMessageHandlerFactory()
        {
            var messageHandlerFactory = new UnityFactory();
            messageHandlerFactory.RegisterMessageHandlers(Assembly.GetExecutingAssembly());
            return messageHandlerFactory;
        }        
    }
}
using System.Reflection;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class ScenarioTestProcessor : MessageProcessor
    {
        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var messageHandlerFactory = new UnityFactory();
            messageHandlerFactory.RegisterMessageHandlers(Assembly.GetExecutingAssembly().GetTypes());
            return messageHandlerFactory;
        }        
    }
}
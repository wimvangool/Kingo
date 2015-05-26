using System.Reflection;

namespace System.ComponentModel.Server
{
    internal sealed class ScenarioTestProcessor : MessageProcessor
    {                
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var messageHandlerFactory = new UnityFactory();
            messageHandlerFactory.RegisterMessageHandlers(Assembly.GetExecutingAssembly());
            return messageHandlerFactory;
        }        
    }
}

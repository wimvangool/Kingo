using Kingo.Messaging;

namespace Kingo.Samples.Chess
{
    internal sealed class WriteOnlyProcessor : MessageProcessor
    {
        #region [====== MessageHandlerFactory ======]        

        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers("*.Application.dll");
            factory.RegisterRepositories("*.Domain.dll", "*.Chess.Tests.dll");
            return factory;
        }

        #endregion
    }
}

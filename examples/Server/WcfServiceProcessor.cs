using System.Collections.Generic;
using Kingo.Messaging;
using Kingo.Transactions;

namespace Kingo.Samples.Chess
{
    public abstract class WcfServiceProcessor : MessageProcessor
    {
        #region [====== Pipeline Configuration ======]

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            yield return new FaultExceptionModule();
            yield return new MessageValidationModule();
            yield return new TransactionScopeModule();
        }

        #endregion

        #region [====== MessageHandlerFactory ======]
        
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers("*.Application.dll", "*.DataAccess.dll");
            factory.RegisterRepositories("*.Domain.dll", "*.DataAccess.dll");
            return factory;
        }

        #endregion        
    }
}

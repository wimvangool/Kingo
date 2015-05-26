namespace System.ComponentModel.Server
{
    internal sealed class UnityTestProcessor : MessageProcessor
    {                
        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            return new UnityFactory();
        }        
    }
}

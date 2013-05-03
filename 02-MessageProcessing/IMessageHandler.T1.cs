
namespace YellowFlare.MessageProcessing
{          
    internal interface IMessageHandler<in TMessage> where TMessage : class
    {
        object Handler
        {
            get;
        }
        
        void Handle(TMessage message);
    }
}

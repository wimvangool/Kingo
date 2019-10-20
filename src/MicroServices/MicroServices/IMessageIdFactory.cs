namespace Kingo.MicroServices
{
    internal interface IMessageIdFactory
    {
        string GenerateMessageIdFor(object message);
    }
}

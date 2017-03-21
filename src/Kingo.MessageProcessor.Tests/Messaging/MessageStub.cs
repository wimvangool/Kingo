using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    internal sealed class MessageStub : Message
    {
        public override Message Copy()
        {
            return new MessageStub();
        }
    }
}

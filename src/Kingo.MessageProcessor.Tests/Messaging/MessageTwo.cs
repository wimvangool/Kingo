using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    internal sealed class MessageTwo : Message
    {
        public override Message Copy()
        {
            return new MessageTwo();
        }
    }
}

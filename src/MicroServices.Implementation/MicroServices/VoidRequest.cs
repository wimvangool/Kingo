using Kingo.Reflection;

namespace Kingo.MicroServices
{
    [Message(MessageKind.Request)]
    internal sealed class VoidRequest
    {
        public override string ToString() =>
            typeof(void).FriendlyName();
    }
}

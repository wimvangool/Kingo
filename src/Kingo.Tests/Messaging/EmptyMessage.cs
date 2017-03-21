using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    [DataContract]
    public sealed class EmptyMessage : Message { }
}

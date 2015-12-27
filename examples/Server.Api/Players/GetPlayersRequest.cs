using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Players
{
    [DataContract]
    public sealed class GetPlayersRequest : Message<GetPlayersRequest> { }
}

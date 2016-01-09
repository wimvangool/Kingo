using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GetActiveGamesRequest : Message<GetActiveGamesRequest> { }
}

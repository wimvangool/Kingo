using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class GetPendingChallengesRequest : Message<GetPendingChallengesRequest> { }
}

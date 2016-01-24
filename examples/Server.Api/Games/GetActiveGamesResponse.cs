using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GetActiveGamesResponse : Message
    {
        [DataMember]
        public readonly ActiveGame[] Games;

        public GetActiveGamesResponse(IEnumerable<ActiveGame> games)
        {
            Games = games.ToArray();
        }
    }
}

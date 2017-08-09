using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class GetActiveGamesResponse : RequestMessage
    {        
        public GetActiveGamesResponse(IEnumerable<ActiveGame> games)
        {
            Games = games.ToArray();
        }

        [DataMember]
        public ActiveGame[] Games
        {
            get;
            private set;
        }
    }
}

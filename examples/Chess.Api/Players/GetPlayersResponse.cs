﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Players
{
    [DataContract]
    public sealed class GetPlayersResponse : Message
    {        
        public GetPlayersResponse(IEnumerable<RegisteredPlayer> players)
        {
            Players = players.ToArray();
        }

        [DataMember]
        public RegisteredPlayer[] Players
        {
            get;
            private set;
        }
    }
}
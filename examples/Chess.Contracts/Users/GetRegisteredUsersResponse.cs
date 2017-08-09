using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Users
{
    [DataContract]
    public sealed class GetRegisteredUsersResponse : RequestMessage
    {        
        public GetRegisteredUsersResponse(IEnumerable<RegisteredUser> players)
        {
            Players = players.ToArray();
        }

        [DataMember]
        public RegisteredUser[] Players
        {
            get;
            private set;
        }
    }
}

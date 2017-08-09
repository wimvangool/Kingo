using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PlayerChallengedEvent : ChallengeEvent
    {               
        public PlayerChallengedEvent(Guid senderId, Guid receiverId)        
        {            
            SenderId = senderId;
            ReceiverId = receiverId;
        }        

        [DataMember]
        public Guid SenderId
        {
            get;
            private set;
        }

        [DataMember]
        public Guid ReceiverId
        {
            get;
            private set;
        }
    }
}

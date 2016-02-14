using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Players
{
    [DataContract]
    public sealed class RegisteredPlayer
    {        
        public RegisteredPlayer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [DataMember]
        public Guid Id
        {
            get;
            private set;
        }

        [DataMember]
        public string Name
        {
            get;
            private set;
        }
    }
}

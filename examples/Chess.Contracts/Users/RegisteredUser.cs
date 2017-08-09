using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Users
{
    [DataContract]
    public sealed class RegisteredUser
    {        
        public RegisteredUser(Guid id, string name)
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

using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Players
{
    [DataContract]
    public sealed class RegisteredPlayer
    {
        [DataMember]
        public readonly Guid Id;

        [DataMember]
        public readonly string Name;

        public RegisteredPlayer(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

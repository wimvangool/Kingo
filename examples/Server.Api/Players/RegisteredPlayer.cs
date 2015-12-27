using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Players
{
    [DataContract]
    public sealed class RegisteredPlayer
    {
        [DataMember]
        public readonly string Name;

        public RegisteredPlayer(string name)
        {
            Name = name;
        }
    }
}

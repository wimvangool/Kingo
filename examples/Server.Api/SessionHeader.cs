using System.Runtime.Serialization;

namespace Kingo.Samples.Chess
{
    [DataContract]
    public sealed class SessionHeader
    {
        public static readonly string Name = typeof(Session).Name;
        public const string Namespace = @"Kingo.Samples.Chess";

        [DataMember]
        public readonly string PlayerName;

        public SessionHeader(string playerName)
        {
            PlayerName = playerName;
        }
    }
}

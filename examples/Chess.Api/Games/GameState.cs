using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public enum GameState
    {
        [EnumMember]
        Normal,

        [EnumMember]
        Check,

        [EnumMember]
        CheckMate,

        [EnumMember]
        StaleMate,

        [EnumMember]
        Forfeited,

        Error
    }
}

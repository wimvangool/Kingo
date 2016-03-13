using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public enum TypeOfPiece
    {
        [EnumMember]
        Pawn,

        [EnumMember]
        Rook,

        [EnumMember]
        Knight,

        [EnumMember]
        Bishop,

        [EnumMember]
        Queen,

        [EnumMember]
        King
    }
}

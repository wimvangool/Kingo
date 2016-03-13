using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

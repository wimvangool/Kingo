using System;
using System.Runtime.Serialization;
using Kingo.Samples.Chess.Users;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class ActiveGame
    {        
        public ActiveGame(Guid gameId, RegisteredUser whitePlayer, RegisteredUser blackPlayer)
        {
            GameId = gameId;
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
        }

        [DataMember]
        public Guid GameId
        {
            get;
            private set;
        }

        [DataMember]
        public RegisteredUser WhitePlayer
        {
            get;
            private set;
        }

        [DataMember]
        public RegisteredUser BlackPlayer
        {
            get;
            private set;
        }
    }
}

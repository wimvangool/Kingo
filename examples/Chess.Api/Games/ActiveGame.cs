using System;
using System.Runtime.Serialization;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class ActiveGame
    {        
        public ActiveGame(Guid gameId, RegisteredPlayer whitePlayer, RegisteredPlayer blackPlayer)
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
        public RegisteredPlayer WhitePlayer
        {
            get;
            private set;
        }

        [DataMember]
        public RegisteredPlayer BlackPlayer
        {
            get;
            private set;
        }
    }
}

using System;
using System.Runtime.Serialization;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class ActiveGame
    {
        [DataMember]
        public readonly Guid GameId;

        [DataMember]
        public readonly RegisteredPlayer WhitePlayer;

        [DataMember]
        public readonly RegisteredPlayer BlackPlayer;

        public ActiveGame(Guid gameId, RegisteredPlayer whitePlayer, RegisteredPlayer blackPlayer)
        {
            GameId = gameId;
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
        }
    }
}

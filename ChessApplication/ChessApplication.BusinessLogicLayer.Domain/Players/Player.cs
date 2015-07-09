using System;
using SummerBreeze.ChessApplication.Challenges;
using Syztem;
using Syztem.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Players
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public sealed class Player : AggregateRoot<Guid, DateTimeOffset>
    {
        private readonly Guid _id;
        private readonly Username _username;
        private readonly Password _password;    
        private DateTimeOffset _version;            
        
        private Player(Guid id, Username username, Password password)
        {
            _id = id;            
            _version = DateTimeOffset.MinValue;
            _username = username;
            _password = password;
        } 
       
        public static Player Register(Guid playerId, Username username, Password password)
        {
            var player = new Player(playerId, username, password);

            player.Publish((id, version) => new PlayerRegisteredEvent(id, version)
            {
                Username = username.ToString()
            });
            return player;
        }

        #region [====== Id & Version ======]

        /// <inheritdoc />
        public override Guid Id
        {
            get { return _id; }
        }

        /// <inheritdoc />
        protected override DateTimeOffset Version
        {
            get { return _version; }
            set { SetVersion(ref _version, value); }
        }

        /// <inheritdoc />
        protected override DateTimeOffset NewVersion()
        {
            return Clock.Current.UtcDateAndTime();
        }

        #endregion

        public Challenge ChallengeOtherPlayer(Guid challengeId, Player receiver)
        {
            return Challenge.CreateChallenge(challengeId, Id, receiver.Id);
        }
    }
}
using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Players
{
    [Serializable]    
    public sealed class Player : AggregateRoot<Guid, int>
    {
        private readonly Guid _id;
        private readonly Identifier _name; 
        private int _version;                       

        private Player(Guid id, int version, Identifier name)
            : base(new PlayerRegisteredEvent(id, version, name))
        {
            _id = id;
            _name = name;
            _version = version;
        }

        public override Guid Id
        {
            get { return _id; }
        }    
    
        public Identifier Name
        {
            get { return _name; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value;}
        }   

        public Challenge Challenge(Guid challengeId, Player other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            if (Id == other.Id)
            {
                throw NewCannotChallengeYourselfException(Name);
            }
            return new Challenge(new PlayerChallengedEvent(challengeId, 1, Id, other.Id));
        }        

        public static Player Register(Guid id, Identifier name)
        {
            return new Player(id, 1, name);
        }

        private static Exception NewCannotChallengeYourselfException(Identifier playerName)
        {
            var messageFormat = ExceptionMessages.Players_PlayerCannotChallengeHimself;
            var message = string.Format(messageFormat, playerName);
            return new DomainException(message);
        }
    }
}

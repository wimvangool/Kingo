using System;
using Kingo.Messaging.Domain;
using Kingo.Messaging.Validation;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Users
{    
    public sealed class User : AggregateRoot<Guid, int>
    {        
        private readonly Identifier _name;         

        private User(UserRegisteredEvent @event)
            : base(@event)
        {            
            _name = Identifier.Parse(@event.UserName);           
        }

        public Identifier Name =>
            _name;

        protected override int NextVersion() =>
            Version + 1;

        protected override ISnapshot<Guid, int> TakeSnapshot()
        {
            throw new NotImplementedException();
        }        

        public Challenge Challenge(Guid challengeId, User other)
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

        public static User Register(Guid id, string name) =>
            new User(new UserRegisteredEvent(id, 1, name));

        private static Exception NewCannotChallengeYourselfException(Identifier playerName)
        {
            var messageFormat = ExceptionMessages.Players_PlayerCannotChallengeHimself;
            var message = string.Format(messageFormat, playerName);
            return new IllegalOperationException(message);
        }
    }
}

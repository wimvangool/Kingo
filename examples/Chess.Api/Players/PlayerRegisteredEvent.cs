using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Players
{
    /// <summary>
    /// This event is raised when a new player has been registered.
    /// </summary>
    [DataContract]
    public sealed class PlayerRegisteredEvent : DomainEvent<Guid, int>
    {                                
        public PlayerRegisteredEvent(Guid playerId, int version, string playerName)
        {
            PlayerId = playerId;
            PlayerVersion = version;
            PlayerName = playerName;            
        }

        [DataMember]
        public Guid PlayerId
        {
            get;
            private set;
        }

        [DataMember]
        public int PlayerVersion
        {
            get;
            private set;
        }

        [DataMember]
        public string PlayerName
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<PlayerRegisteredEvent>();

            validator.VerifyThat(m => m.PlayerId).IsNotEmpty();
            validator.VerifyThat(m => m.PlayerVersion).IsEqualTo(1);
            validator.VerifyThat(m => m.PlayerName).IsNotNull().IsIdentifier();            

            return validator;
        }
    }
}

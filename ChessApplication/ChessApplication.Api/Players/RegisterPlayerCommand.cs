using System;
using System.Runtime.Serialization;
using Kingo.BuildingBlocks;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.ChessApplication.Resources;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Represents a request to register a new player.
    /// </summary>
    [DataContract]
    public sealed class RegisterPlayerCommand : Message<RegisterPlayerCommand>
    {
        [DataMember(IsRequired = true)]
        public readonly Guid PlayerId;

        [DataMember(IsRequired = true)]
        public readonly string Username;

        [DataMember(IsRequired = true)]
        public readonly string Password;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPlayerCommand" /> class.
        /// </summary>
        /// <param name="username">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        public RegisterPlayerCommand(string username, string password)
        {
            PlayerId = Guid.NewGuid();
            Username = username;
            Password = password;
        }

        #region [====== Copy ======]

        private RegisterPlayerCommand(RegisterPlayerCommand message)
            : base(message)
        {
            PlayerId = message.PlayerId;
            Username = message.Username;
            Password = message.Password;
        }

        public override RegisterPlayerCommand Copy()
        {
            return new RegisterPlayerCommand(this);
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        public override bool Equals(object obj)
        {
            return Equals(obj as RegisterPlayerCommand);
        }

        public bool Equals(RegisterPlayerCommand other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(Username, other.Username) && Equals(Password, other.Password);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(Username, Password);
        }

        #endregion

        #region [====== Validation ======]

        protected override IMessageValidator CreateValidator()
        {
            var validator = new ConstraintValidator();            

            VerifyUsername(validator, Constraints.UsernameMinLength, Constraints.UsernameMaxLength, Constraints.UsernameRegex);
            VerifyPassword(validator, Constraints.PasswordMinLength, Constraints.PasswordMaxLength, Constraints.PasswordRegex);            

            return validator;
        }        

        private void VerifyUsername(IMemberConstraintSet validator, int minLength, int maxLength, string regex)
        {
            validator.VerifyThat(() => Username)
                .IsNotNullOrWhiteSpace(ValidationErrorMessages.RegisterPlayerCommand_Username_NotSpecified)
                .HasLengthBetween(minLength, maxLength, ValidationErrorMessages.RegisterPlayerCommand_Username_InvalidLength)
                .Matches(regex, ValidationErrorMessages.RegisterPlayerCommand_Username_IllegalCharacters);
        }

        private void VerifyPassword(IMemberConstraintSet validator, int minLength, int maxLength, string regex)
        {
            validator.VerifyThat(() => Password)
                .IsNotNullOrWhiteSpace(ValidationErrorMessages.RegisterPlayerCommand_Password_NotSpecified)
                .HasLengthBetween(minLength, maxLength, ValidationErrorMessages.RegisterPlayerCommand_Password_InvalidLength)
                .Matches(regex, ValidationErrorMessages.RegisterPlayerCommand_Password_IllegalCharacters);
        }

        #endregion
    }
}
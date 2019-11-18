using System.ComponentModel.DataAnnotations;

namespace Kingo.MasterMind.GameService
{
    /// <summary>
    /// Represents a <see cref="ValidationAttribute"/> that can be used to validate a player-name property.
    /// </summary>
    public sealed class PlayerNameAttribute : RequestMessageValidationAttribute
    {
        private readonly RegularExpressionAttribute _regularExpressionAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerNameAttribute" /> class.
        /// </summary>
        public PlayerNameAttribute() :
            base(nameof(ErrorMessages.StartGameCommand_InvalidPlayerName))
        {
            _regularExpressionAttribute = new RegularExpressionAttribute("[a-zA-Z]+");
        }

        /// <inheritdoc />
        public override bool IsValid(object value) =>
            _regularExpressionAttribute.IsValid(value);
    }
}

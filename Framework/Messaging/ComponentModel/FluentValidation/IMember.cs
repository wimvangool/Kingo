namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// When implemented by a class, represent a certain member that can be validated and produces an
    /// <see cref="FormattedString" /> if this validation fails.
    /// </summary>
    public interface IMember : IErrorMessageProducer
    {
        /// <summary>
        /// The full name of this member.
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// The name of this member.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// The value of this member.
        /// </summary>
        object Value
        {
            get;
        }
    }
}

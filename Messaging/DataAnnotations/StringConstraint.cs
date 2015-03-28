namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Represents a value that can be used on the <see cref="RequiredMemberAttribute" /> to indicate
    /// what values of a string are interpreted as invalid.
    /// </summary>
    public enum StringConstraint
    {
        /// <summary>
        /// Indicates that a string may not be <c>null</c>.
        /// </summary>
        NotNull,

        /// <summary>
        /// Indicates that a string may not be <c>null</c> or the <see cref="string.Empty">empty string</see>.
        /// </summary>
        NotNullOrEmpty,

        /// <summary>
        /// Indicates that a string may not be <c>null</c>, the <see cref="string.Empty">empty string</see>,
        /// or contain only white space.
        /// </summary>
        NotNullOrWhiteSpace
    }
}

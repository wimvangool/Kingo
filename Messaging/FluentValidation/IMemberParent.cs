namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// When implemented by a class, represent a <see cref="IMember" />'s parent that keeps track of
    /// all its members based on their name.
    /// </summary>
    public interface IMemberParent
    {
        /// <summary>
        /// Replaces one member instance by another based on its name.
        /// </summary>
        /// <param name="memberName">Name of the member to replace.</param>
        /// <param name="oldProducer">The old member instance.</param>
        /// <param name="newProducer">The new member instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception>
        void ReplaceMember(string memberName, IErrorMessageProducer oldProducer, IErrorMessageProducer newProducer);
    }
}

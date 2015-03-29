using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, this type is able to create a <see cref="IDomainEventList" />
    /// and pass it to a specified handler.
    /// </summary>
    public interface IDomainEventListFactory
    {
        /// <summary>
        /// Passes all published events in the form of a <see cref="IDomainEventList" /> to
        /// the specified <paramref name="domainEventListHandler"/>.
        /// </summary>
        /// <param name="domainEventListHandler">A handler.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="domainEventListHandler"/> is <c>null</c>.
        /// </exception>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By Design")]
        void And(Action<IDomainEventList> domainEventListHandler);
    }
}

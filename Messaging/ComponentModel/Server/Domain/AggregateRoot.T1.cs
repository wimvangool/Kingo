using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and uses a <see cref="DateTime">timestamp</see> as its version.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate-key.</typeparam>    
    [Serializable]
    public abstract class AggregateRoot<TKey> : AggregateRoot<TKey, DateTimeOffset>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TKey}" /> class.
        /// </summary>
        protected AggregateRoot() { }

            /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, K}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AggregateRoot(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Creates and returns a timestamp that is newer than the specified <paramref name="version"/>.
        /// </summary>
        /// <param name="version">The version to increment.</param>
        /// <returns>A timestamp that is newer than the specified <paramref name="version"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="version"/> is newer than the current date and time.
        /// </exception>
        protected override DateTimeOffset Increment(DateTimeOffset version)
        {
            var newVersion = Clock.Current.LocalDateAndTime();            
            if (newVersion <= version)
            {
                throw NewInvalidVersionException(version, newVersion);
            }
            return newVersion;
        }

        private static Exception NewInvalidVersionException(DateTimeOffset version, DateTimeOffset newVersion)
        {
            var messageFormat = ExceptionMessages.AggregateRoot_InvalidTimestampVersion;
            var message = string.Format(messageFormat, version, newVersion);
            return new ArgumentOutOfRangeException("version", message);
        }
    }
}

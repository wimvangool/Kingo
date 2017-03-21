using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// Contains all configuration settings for a <see cref="IMessageHandler{T}" />.
    /// </summary>
    [Serializable]
    public struct MessageHandlerConfiguration : IEquatable<MessageHandlerConfiguration>, IMessageHandlerConfiguration
    {
        /// <summary>
        /// The default configuration that is applied for message handlers.
        /// </summary>
        public static readonly MessageHandlerConfiguration Default = new MessageHandlerConfiguration(MessageHandlerLifetime.PerUnitOfWork);        

        /// <summary>
        /// Initializes a new instance of a <see cref="MessageHandlerConfiguration" /> structure.
        /// </summary>
        /// <param name="lifetime">The lifetime of the <see cref="IMessageHandler{T}" />.</param>
        public MessageHandlerConfiguration(MessageHandlerLifetime lifetime)
        {
            Lifetime = lifetime;
            Sources = MessageSources.InputStream;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="MessageHandlerConfiguration" /> structure.
        /// </summary>
        /// <param name="lifetime">The lifetime of the <see cref="IMessageHandler{T}" />.</param>
        /// <param name="sources">Specifies which source(s) the message is accepted from.</param>
        public MessageHandlerConfiguration(MessageHandlerLifetime lifetime, MessageSources sources)
        {
            Lifetime = lifetime;
            Sources = sources;
        }

        /// <inheritdoc />
        public MessageHandlerLifetime Lifetime
        {
            get;
        }

        /// <inheritdoc />
        public MessageSources Sources
        {
            get;
        }

        #region [====== Object Identity ======]

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is an instance of <see cref="MessageHandlerConfiguration" />
        /// and equals the value of this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is MessageHandlerConfiguration)
            {
                return Equals((MessageHandlerConfiguration) obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="MessageHandlerConfiguration" /> value.
        /// </summary>
        /// <param name="other">A <see cref="MessageHandlerConfiguration" /> value to compare to this instance.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MessageHandlerConfiguration other) =>
            Lifetime.Equals(other.Lifetime) && Sources.Equals(other.Sources);

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() =>
            Lifetime.GetHashCode() ^ Sources.GetHashCode();

        #endregion

        #region [====== Formatting and Parsing ======]

        /// <summary>Converts this value to its equivalent string-representation.</summary>
        /// <returns>The string-representation of this value.</returns>
        public override string ToString() =>
            $"[{Sources}, {Lifetime}]";

        #endregion

        #region [====== Operator Overloads ======]

        /// <summary>Determines whether two specified <see cref="MessageHandlerConfiguration" />-instances have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances have the same value; otherwise <c>false</c>.</returns>
        public static bool operator ==(MessageHandlerConfiguration left, MessageHandlerConfiguration right) =>
            left.Equals(right);

        /// <summary>Determines whether two specified <see cref="MessageHandlerConfiguration" />-instances do not have the same value.</summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare</param>
        /// <returns><c>true</c> if both instances do not have the same value; otherwise <c>false</c>.</returns>
        public static bool operator !=(MessageHandlerConfiguration left, MessageHandlerConfiguration right) =>
            !left.Equals(right);

        #endregion
    }
}
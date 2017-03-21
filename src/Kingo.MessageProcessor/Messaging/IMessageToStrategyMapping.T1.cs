using System;
using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a mapping from specific <see cref="IMessage">Messages</see>
    /// to certain strategies to use inside a <see cref="IMicroProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TStrategy"></typeparam>
    public interface IMessageToStrategyMapping<TStrategy> where TStrategy : class
    {        
        /// <summary>
        /// Attempts to retrieve a mapped <paramref name="strategy"/> for the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="strategy">
        /// If a mapping is found, this parameter will refer to the mapped strategy when the method returns;
        /// otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if a mapping exists for <paramref name="message"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        bool TryGetStrategy(IMessage message, out TStrategy strategy);

        /// <summary>
        /// Attempts to retrieve a mapped <paramref name="strategy"/> for the specified <paramref name="messageType"/>.
        /// </summary>
        /// <param name="messageType">A message type.</param>
        /// <param name="strategy">
        /// If a mapping is found, this parameter will refer to the mapped strategy when the method returns;
        /// otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if a mapping exists for <paramref name="messageType"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        bool TryGetStrategy(Type messageType, out TStrategy strategy);        
    }
}

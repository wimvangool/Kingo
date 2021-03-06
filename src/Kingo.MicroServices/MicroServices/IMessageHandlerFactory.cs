﻿using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a service provider that specializes in resolving <see cref="MessageHandler"/>
    /// instances for a specific message.
    /// </summary>
    public interface IMessageHandlerFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IServiceProvider" /> that can be used to resolve all message-handlers
        /// and their dependencies. This method can be used to obtain a <see cref="IServiceProvider"/> if no provider
        /// was injected into the constructor of the associated <see cref="MicroProcessor" />.
        /// </summary>
        /// <returns>A new service-provider.</returns>
        IServiceProvider CreateServiceProvider();

        /// <summary>
        /// Resolves all message handlers for the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the specified <paramref name="message"/>.</typeparam>
        /// <param name="message">Message for which the handlers are resolved.</param>
        /// <param name="context">Context in which the message handlers will be invoked.</param>
        /// <returns>A collection of message handlers that will be invoked for the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        IEnumerable<MessageHandler> ResolveMessageHandlers<TMessage>(TMessage message, MessageHandlerContext context);        
    }
}

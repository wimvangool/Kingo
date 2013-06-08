﻿using System;

namespace YellowFlare.MessageProcessing
{    
    /// <summary>
    /// When implemented by a class, handles messages of the specified <paramtyperef name="TMessage" />.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>  
    public interface IMessageHandler<in TMessage> where TMessage : class
    {        
        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Handle(TMessage message);
    }
}

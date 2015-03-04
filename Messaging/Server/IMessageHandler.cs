﻿namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IMessageHandler{TMessage}" /> that is
    /// ready to be invoked with a specific <see cref="Message" />.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// The message that will be passed to the <see cref="IMessageHandler{TMessage}" />.
        /// </summary>
        IMessage Message
        {
            get;
        }

        /// <summary>
        /// Invokes the underlying  <see cref="IMessageHandler{TMessage}" /> with the <see cref="Message" />.
        /// </summary>
        void Invoke();
    }
}
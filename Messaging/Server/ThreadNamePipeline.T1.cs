using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// This pipeline changes the name of the current thread so that it can easily be identified in the debugger.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    public class ThreadNamePipeline<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadNamePipeline{TMessage}" /> class.
        /// </summary>
        /// <param name="handler">The next handler to invoke.</param>               
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public ThreadNamePipeline(IMessageHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }            
            _handler = handler;
        }

        /// <inheritdoc />
        public void Handle(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var currentThread = Thread.CurrentThread;
            if (currentThread.Name == null)
            {
                currentThread.Name = CreateThreadName(currentThread);
            }

            _handler.Handle(message);
        }

        /// <summary>
        /// Creates and returns an appropriate name for the specified <paramref name="thread"/>.
        /// </summary>
        /// <param name="thread">The thread to specify the name for.</param>        
        /// <returns>An appropriate name.</returns>
        protected virtual string CreateThreadName(Thread thread)
        {
            return string.Format("MessageProcessorThread ({0})", thread.ManagedThreadId);
        }
    }
}

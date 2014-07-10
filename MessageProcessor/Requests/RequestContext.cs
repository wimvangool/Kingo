using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a context that is used to sends or posts messages to the appropriate thread(s) using a <see cref="SynchronizationContext" />.
    /// </summary>
    public sealed class RequestContext
    {
        private readonly SynchronizationContext _context;        
        
        private RequestContext(SynchronizationContext context)
        {
            _context = context;
        }     
   
        /// <summary>
        /// Indicates whether or not this context is associated to a <see cref="SynchronizationContext" /> such that
        /// it can send or post any messages to it.
        /// </summary>
        public bool IsAssociatedToSynchronizationContext
        {
            get { return _context != null; }
        }

        /// <summary>
        /// If associated to a <see cref="SynchronizationContext" />, sends a (synchronous) message to the appropriate thread.
        /// </summary>
        /// <param name="action">The message to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_context != null)
            {
                _context.Send(state => action.Invoke(), null);
            }            
        }

        /// <summary>
        /// If associated to a <see cref="SynchronizationContext" />, posts an (asynchronous) message to the appropriate thread.
        /// </summary>
        /// <param name="action">The message to post.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        public void InvokeAsync(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (_context != null)
            {
                _context.Post(state => action.Invoke(), null);
            }           
        }

        internal static RequestContext NewContext()
        {
            return new RequestContext(SynchronizationContext.Current);
        }
    }
}

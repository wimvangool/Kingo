namespace System.ComponentModel
{
    /// <summary>
    /// Contains extension-methods for raising events.
    /// </summary>
    public static class EventHandlerExtensions
    {
        #region [====== Instance Events ======]

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using empty arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> is <c>null</c>.
        /// </exception>
        public static void Raise(this EventHandler handlers, object sender)
        {
            Raise(handlers, sender, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void Raise(this EventHandler handlers, object sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handlers, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void Raise<TEventArgs>(this Delegate handlers, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.DynamicInvoke(sender, e);
            }
        }

        #endregion

        #region [====== Static Events ======]

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using empty arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>
        public static void RaiseStatic(this EventHandler handlers)
        {
            Raise(handlers, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>        
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void RaiseStatic(this EventHandler handlers, EventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.Invoke(null, e);
            }
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>        
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void RaiseStatic<TEventArgs>(this EventHandler<TEventArgs> handlers, TEventArgs e) where TEventArgs : EventArgs
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.Invoke(null, e);
            }
        }

        /// <summary>
        /// Invokes the specified <paramref name="handlers"/>, if not <c>null</c>, using the specified arguments.
        /// </summary>
        /// <param name="handlers">The handlers to invoke.</param>        
        /// <param name="e">The arguments of the event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e"/> is <c>null</c>.
        /// </exception>
        public static void RaiseStatic<TEventArgs>(this Delegate handlers, TEventArgs e) where TEventArgs : EventArgs
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (handlers != null)
            {
                handlers.DynamicInvoke(null, e);
            }
        }

        #endregion
    }
}
